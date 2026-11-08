using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using MinionLib.Utilities;
using TheTailor.BaseLibAdapters;

namespace TheTailor.Minions;

public abstract class TailorMinion : CustomMinionModel
{
    public override string? HurtSfx => "res://TheTailor/audio/clothHit1.ogg";
    public override float DeathAnimLengthOverride => 0.8f;
    public override float HpBarSizeReduction => 12f;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.Monster == this) SoundEffects.MinionDeath.Play();
    }
}

/// <summary>
///     Fixes Paper Cuts not reducing minions' nor players' max HP
/// </summary>
[HarmonyPatch]
internal static class MinionPapercutsPatch
{
    [HarmonyPatch(typeof(PaperCutsPower), "AfterDamageGiven")]
    internal static async void Postfix(PaperCutsPower __instance, PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != __instance.Owner || target is not { IsPlayer: false, PetOwner: not null } ||
            !props.IsPoweredAttack() ||
            result.UnblockedDamage <= 0) return;
        await CreatureCmd.LoseMaxHp(choiceContext, target, __instance.Amount, false);
        if (result.OverkillDamage <= 0) return;
        var accessor = new PetsOrderAccessor(target.PetOwner);
        var indexOfPet = accessor.Pets.IndexOf(target);
        if (indexOfPet == accessor.Pets.Count - 1)
            await CreatureCmd.LoseMaxHp(choiceContext, target.PetOwner.Creature, __instance.Amount, false);
        else if (accessor.Pets.Count - 1 > indexOfPet)
            await CreatureCmd.LoseMaxHp(choiceContext, accessor.Pets[indexOfPet + 1], __instance.Amount, false);
    }
}

/// <summary>
///     Makes Minions consume Buffer
/// </summary>
[HarmonyPatch]
internal static class MinionBufferPatch
{
    [HarmonyPatch(typeof(BufferPower), "ModifyHpLostAfterOstyLate")]
    internal static decimal Postfix(decimal __result, Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, BufferPower __instance)
    {
        if (__instance.Owner.Pets.Contains(target) && target.Monster is TailorMinion) return 0m;

        return __result;
    }
}

/// <summary>
///     Makes Minions consume Intangible
/// </summary>
[HarmonyPatch]
internal static class MinionIntangiblePatch
{
    [HarmonyPatch(typeof(IntangiblePower), "ModifyHpLostAfterOsty")]
    internal static decimal Postfix(decimal __result, Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, BufferPower __instance)
    {
        if (__instance.Owner.Pets.Contains(target) && target.Monster is TailorMinion) return 1m;

        return __result;
    }
}

/// <summary>
///     Enables the selection reticle when a Minion is hovered over to make its interactability obvious
/// </summary>
[HarmonyPatch]
internal static class MinionReticlePatch
{
    [HarmonyPatch(typeof(NCreature), "OnFocus")]
    internal static void Postfix(NCreature __instance)
    {
        if (__instance.Entity.Monster is TailorMinion)
            __instance.ShowSingleSelectReticle();
    }
}

/// <summary>
///     Makes the death fade vfx unclickable so Minions are easier to re-order during the animation
/// </summary>
[HarmonyPatch]
internal static class MinionDeathClickablePatch
{
    [HarmonyPatch(typeof(NCreature), "DisableInteractionForDeath")]
    internal static void Postfix(NCreature __instance)
    {
        if (__instance.Entity.Monster is not TailorMinion) return;
        foreach (var control in __instance.Visuals.GetChildrenRecursive<Control>())
        {
            if (control.HasFocus()) ActiveScreenContext.Instance.FocusOnDefaultControl();
            control.FocusMode = Control.FocusModeEnum.None;
            control.MouseFilter = Control.MouseFilterEnum.Ignore;
        }
    }
}