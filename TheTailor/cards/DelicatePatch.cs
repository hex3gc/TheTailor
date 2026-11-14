using Godot;
using HarmonyLib;
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
                cardPlay.Card.AddKeyword(CardKeyword.Exhaust);
        }
    }
}

/// <summary>
///     Hides the 'Exhaust' keyword when a card has Delicate
/// </summary>
[HarmonyPatch]
internal static class DelicateHideExhaustPatch
{
    [HarmonyPatch(typeof(CardModel), "GetDescriptionForPile", typeof(PileType),
        typeof(CardModel.DescriptionPreviewType), typeof(Creature))]
    internal static string Postfix(string __result, PileType pileType, CardModel.DescriptionPreviewType previewType,
        Creature? target, CardModel __instance)
    {
        if (__instance.DynamicVars.ContainsKey("Delicate") && __instance.DynamicVars["Delicate"].BaseValue > -999 &&
            __instance.Keywords.Contains(CardKeyword.Exhaust))
        {
            var exhaustString = "\n" + CardKeyword.Exhaust.GetCardText();
            var exhaustStringIndex = __result.Find(exhaustString);
            if (exhaustStringIndex > -1) __result = __result.Remove(exhaustStringIndex, exhaustString.Length);
        }

        return __result;
    }
}