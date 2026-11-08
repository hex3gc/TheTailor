using BaseLib.Abstracts;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace TheTailor.Powers;

public sealed class SteadyHandPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/steadyHandSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/steadyHand.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/steadyHand.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner == Owner && power is SteadyHandPower && Owner.IsPlayer)
        {
            var enumerable = Owner.Player.PlayerCombatState.AllCards.Where(c => c.DynamicVars.ContainsKey("Delicate"));
            foreach (var card in enumerable)
            {
                if (card.DynamicVars["Delicate"].IntValue + Amount > 1 && card.Keywords.Contains(CardKeyword.Exhaust))
                    card.RemoveKeyword(CardKeyword.Exhaust);
                card.UpdateDynamicVarPreview(CardPreviewMode.Normal, card.Owner.Creature, card.DynamicVars);
            }
        }
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner.HasPower<SteadyHandPower>() && card.DynamicVars.ContainsKey("Delicate"))
        {
            if (card.DynamicVars["Delicate"].IntValue + Amount > 1 && card.Keywords.Contains(CardKeyword.Exhaust))
                card.RemoveKeyword(CardKeyword.Exhaust);
            card.UpdateDynamicVarPreview(CardPreviewMode.Normal, card.Owner.Creature, card.DynamicVars);
        }
    }
}

[HarmonyPatch]
internal static class DelicateNumberDisplayPatch
{
    [HarmonyPatch(typeof(DynamicVar), "UpdateCardPreview")]
    internal static async void Postfix(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks, DynamicVar __instance)
    {
        if (__instance.Name == "Delicate" && card.Owner.HasPower<SteadyHandPower>())
            __instance.PreviewValue = __instance.BaseValue + card.Owner.Creature.GetPowerAmount<SteadyHandPower>();
    }
}