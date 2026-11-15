using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Powers;

#pragma warning disable CS8600
namespace TheTailor.Minions;

/// <summary>
///     A clone of MinionGuardianOverkillPatch working on Multiplayer
///     - This failed to make Phantasmal Gardeners give block, so certain things may cause problems with damage calculation
/// </summary>
[HarmonyPatch(typeof(CreatureCmd), "Damage", typeof(PlayerChoiceContext), typeof(IEnumerable<DamageTargetData>),
    typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay), typeof(ModifyDamageHookType))]
public static class TailorMinionOverkillPatch
{
    private static readonly AsyncLocal<bool> IsHandling = new();
    public static readonly AsyncLocal<Creature?> SuppressedOwner = new();

    [HarmonyPrefix]
    private static bool Prefix(PlayerChoiceContext choiceContext, IEnumerable<DamageTargetData> targetData, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay, ModifyDamageHookType modifyDamageHookType,
        ref Task<IEnumerable<DamageResult>> __result)
    {
        var atLeastOneMinion = false;
        foreach (var data in targetData)
            if (data.target.Pets != null && data.target.Pets.Any(c => c.Monster is TailorMinion))
                atLeastOneMinion = true;

        if (!atLeastOneMinion) return true;
        if (IsHandling.Value) return true;
        if (targetData.Count() == 1 && !ShouldHandle(targetData.First().target, props)) return true;
        if (targetData.Any(c => c.target.IsEnemy)) return true;

        var list = targetData.ToList();
        __result = HandleWithOverkillRedirect(choiceContext, list, amount, props, dealer, cardSource, cardPlay);

        return false;
    }

    private static bool ShouldHandle(Creature target, ValueProp props)
    {
        if (!target.IsPlayer || target.Player == null || target.IsDead || target.CombatState == null) return false;

        if (!props.HasFlag(ValueProp.Move) || props.HasFlag(ValueProp.Unpowered)) return false;

        return target.Pets.Any(p => p.IsAlive && IsFrontGuardian(p));
    }

    private static async Task<IEnumerable<DamageResult>> HandleWithOverkillRedirect(PlayerChoiceContext choiceContext,
        IReadOnlyList<DamageTargetData> targets, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        IsHandling.Value = true;
        var ret = new List<DamageResult>();

        foreach (var targetData in targets)
        {
            if (!ShouldHandle(targetData.target, props))
            {
                ret.Concat(await CreatureCmd.Damage(choiceContext, targetData.target, amount, props, dealer, cardSource, cardPlay));
                continue;
            }

            var guardianOrder =
            (
                from p in PetOrderSnapshotManager.GetSnapshot(targetData.target.Player, false)
                where IsFrontGuardian(p) && p.CombatId.HasValue
                select p.CombatId!.Value
            ).ToList();

            SuppressedOwner.Value = targetData.target;
            List<DamageResult> initialResults;
            try
            {
                initialResults =
                    (await CreatureCmd.Damage(choiceContext, targetData.target, amount, props, dealer, cardSource, cardPlay))
                    .ToList();
            }
            finally
            {
                SuppressedOwner.Value = null;
            }

            var damageResult = initialResults.FirstOrDefault(delegate(DamageResult r)
            {
                if (r.Receiver != targetData.target && r.Receiver.PetOwner == targetData.target.Player)
                {
                    if (!IsFrontGuardian(r.Receiver))
                    {
                        var combatId = r.Receiver.CombatId;
                        if (combatId.HasValue)
                        {
                            var valueOrDefault = combatId.GetValueOrDefault();
                            return guardianOrder.Contains(valueOrDefault);
                        }

                        return false;
                    }

                    return true;
                }

                return false;
            });
            if (damageResult == null || damageResult.OverkillDamage <= 0 || !damageResult.Receiver.CombatId.HasValue)
            {
                ret.Concat(initialResults);
                continue;
            }

            var redirectedResults = new List<DamageResult>();
            decimal overkill = damageResult.OverkillDamage;
            var value = damageResult.Receiver.CombatId.Value;
            var directProps = props | ValueProp.Unpowered;
            var num2 = guardianOrder.IndexOf(value);
            if (num2 < 0)
            {
                if (overkill > 0m)
                {
                    var item = (await CreatureCmd.Damage(choiceContext, targetData.target, overkill, directProps, dealer,
                        cardSource, cardPlay)).FirstOrDefault() ?? new DamageResult(targetData.target, directProps);
                    redirectedResults.Add(item);
                }

                initialResults.AddRange(redirectedResults);
                ret.Concat(initialResults);
                continue;
            }

            foreach (var item3 in guardianOrder.Skip(num2 + 1))
            {
                if (!(overkill <= 0m))
                {
                    var defender = targetData.target.CombatState.GetCreature(item3);
                    if (defender != null && defender.IsAlive && IsFrontGuardian(defender))
                    {
                        var damageResult2 =
                            (await CreatureCmd.Damage(choiceContext, defender, overkill, directProps, dealer,
                                cardSource, cardPlay)).FirstOrDefault() ?? new DamageResult(defender, directProps);
                        redirectedResults.Add(damageResult2);
                        overkill = damageResult2.OverkillDamage;
                    }

                    continue;
                }

                break;
            }

            if (overkill > 0m)
            {
                var item2 = (await CreatureCmd.Damage(choiceContext, targetData.target, overkill, directProps, dealer, cardSource,
                    cardPlay)).FirstOrDefault() ?? new DamageResult(targetData.target, directProps);
                redirectedResults.Add(item2);
            }

            initialResults.AddRange(redirectedResults);
            ret.Concat(redirectedResults);
        }

        IsHandling.Value = false;
        return ret;
    }

    private static bool IsFrontGuardian(Creature creature)
    {
        if (creature.GetPower<TailorMadePower>() != null || creature.GetPower<DieForYouPower>() != null) return true;

        return false;
    }
}