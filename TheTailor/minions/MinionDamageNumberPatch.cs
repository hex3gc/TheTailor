using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace TheTailor.Minions;

/// <summary>
///     Like Osty: the damage number above a Tailor minion shows the damage it actually lost, not the overkill.
///     The overkill is passed on to the next minion (or the owner), which shows its own number.
/// </summary>
[HarmonyPatch(typeof(NDamageNumVfx))]
internal static class MinionDamageNumberPatch
{
    [HarmonyPatch(nameof(NDamageNumVfx.Create), typeof(Creature), typeof(DamageResult))]
    [HarmonyPrefix]
    private static bool CreatePrefix(Creature target, DamageResult result, ref NDamageNumVfx? __result)
    {
        if (target.Monster is not TailorMinion) return true;

        if (TestMode.IsOn)
        {
            __result = null;
            return false;
        }

        __result = NDamageNumVfx.Create(target, result.UnblockedDamage);
        return false;
    }
}
