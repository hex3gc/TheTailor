using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Powers;

namespace TheTailor.Cards;

/// <summary>
///     Decrements Delicate on card play and Exhausts cards when it reaches 0
/// </summary>
[HarmonyPatch]
internal static class DelicatePatch
{
    [HarmonyPatch(typeof(AbstractModel), "AfterCardPlayed")]
    internal static async void Postfix(PlayerChoiceContext choiceContext, CardPlay cardPlay, AbstractModel __instance)
    {
        if (__instance is CardModel && cardPlay.Card == __instance && cardPlay.Card.DynamicVars.ContainsKey("Delicate"))
        {
            var extraDelicacies = 0;
            extraDelicacies += cardPlay.Card.Owner.Creature.GetPowerAmount<SteadyHandPower>();

            cardPlay.Card.DynamicVars["Delicate"].BaseValue -= 1;
            if (cardPlay.Card.DynamicVars["Delicate"].IntValue + extraDelicacies <= 1) 
            {
                cardPlay.Card.AddKeyword(CardKeyword.Exhaust);
            }
        }
    }
}

/// <summary>
///     Clamps the Replay count of a card to its Delicate number
/// </summary>
/*
[HarmonyPatch]
internal static class DelicateReplayLimitPatch
{
    [HarmonyPatch(typeof(CardModel), "GetEnchantedReplayCount")]
    internal static int Postfix(int __result, CardModel __instance)
    {
        if (__instance.DynamicVars.ContainsKey("Delicate"))
        {
            return Math.Min(Math.Max(__instance.DynamicVars["Delicate"].IntValue - 1, 0), __result);
        }
        return __result;
    }
}
*/

/// <summary>
///     Exhausts Delicate 0 cards when they move piles to account for Replay jank
/// </summary>
[HarmonyPatch]
internal static class KillDelicateZeroCardsPatch
{
    [HarmonyPatch(typeof(AbstractModel), "AfterCardChangedPiles")]
    internal static async void Postfix(CardModel card, PileType oldPileType, AbstractModel? clonedBy, AbstractModel __instance)
    {
        if (card?.Pile == null)
        {
            return;
        }

        if (card != __instance)
        {
            return;
        }

        var extraDelicacies = 0;
        extraDelicacies += card.Owner.Creature.GetPowerAmount<SteadyHandPower>();

        if ((card.Pile.Type == PileType.Discard || card.Pile.Type == PileType.Hand || card.Pile.Type == PileType.Draw) && card.DynamicVars.ContainsKey("Delicate") && card.DynamicVars["Delicate"].IntValue > -999 && card.DynamicVars["Delicate"].IntValue + extraDelicacies <= 0)
        {
            if (!TailorStitchQueueSingleton.BeingPlayed.Contains(card))
            {
                await CardCmd.Exhaust(new ThrowingPlayerChoiceContext(), card);
            }
        }
    }
}

/// <summary>
///     Hides the 'Exhaust' keyword when a card has Delicate
/// </summary>
[HarmonyPatch]
internal static class DelicateHideExhaustPatch
{
    [HarmonyPatch(typeof(CardModel), "GetDescriptionForPile", typeof(PileType), typeof(CardModel.DescriptionPreviewType), typeof(Creature))]
    internal static string Postfix(string __result, PileType pileType, CardModel.DescriptionPreviewType previewType, Creature? target, CardModel __instance)
    {
        if (__instance.DynamicVars.ContainsKey("Delicate") && __instance.DynamicVars["Delicate"].BaseValue > -999 && __instance.Keywords.Contains(CardKeyword.Exhaust))
        {
            var exhaustString = "\n" + CardKeyword.Exhaust.GetCardText();
            var exhaustStringIndex = __result.Find(exhaustString);
            if (exhaustStringIndex > -1) __result = __result.Remove(exhaustStringIndex, exhaustString.Length);
        }

        return __result;
    }
}