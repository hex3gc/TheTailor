using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace TheTailor.Cards;

public class StitchCardModifier : CardModifier
{
    private CardModel? _stitchedCard;

    public CardModel? StitchedCard
    {
        get => _stitchedCard;
        set
        {
            if (_stitchedCard != null)
            {
                Log.Error("Card cannot be stitched twice");
                return;
            }

            if (value == null)
            {
                Log.Error("Attempt to stitch null card");
                return;
            }

            _stitchedCard = value;
            _stitchedCard.AddKeyword(Keywords.Stitched);
        }
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == Owner && !StitchTrackAutoplaySingleton.BlockedFromAutoplay.Contains(Owner) &&
            Owner != null && StitchedCard != null && cardPlay.PlayIndex == 0)
        {
            TailorStitchQueueSingleton.BeingPlayed.Add(Owner);
            TailorStitchQueueSingleton.BeingPlayed.Add(StitchedCard);

            var target = GetTarget(StitchedCard, StitchedCard.CombatState);
            if (cardPlay.Target != null && cardPlay.Target.IsAlive && target != null &&
                cardPlay.Card.TargetType == StitchedCard.TargetType) target = cardPlay.Target;
            await CardCmd.AutoPlay(choiceContext, StitchedCard, target, StitchedAutoPlayType.Stitched);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        TailorStitchQueueSingleton.BeingPlayed.Remove(cardPlay.Card);

        if (cardPlay.Card != Owner) return;

        if (StitchedCard?.Pile == null || !StitchedCard.IsInCombat)
        {
            if (!TailorStitchQueueSingleton.BeingPlayed.Contains(Owner)) await StitchCmd.UnstitchCard(Owner);
        }
        else if (Owner.Type == CardType.Power || Owner.Pile.Type == PileType.Exhaust ||
                 StitchedCard.Pile.Type == PileType.Exhaust || Owner.Pile.Type == PileType.None ||
                 StitchedCard.Pile.Type == PileType.None)
        {
            if (!TailorStitchQueueSingleton.BeingPlayed.Contains(StitchedCard))
                await StitchCmd.UnstitchCard(StitchedCard);
            if (!TailorStitchQueueSingleton.BeingPlayed.Contains(Owner)) await StitchCmd.UnstitchCard(Owner);
        }
    }

    public static Creature? GetTarget(CardModel card, ICombatState combatState)
    {
        var combatTargets = card.Owner.RunState.Rng.CombatTargets;
        return card.TargetType switch
        {
            TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(),
            TargetType.AnyAlly => combatTargets.NextItem(combatState.Allies.Where(c =>
                c is { IsAlive: true, IsPlayer: true } && c != card.Owner.Creature)),
            TargetType.AnyPlayer => card.Owner.Creature,
            TargetType.Self => card.Owner.Creature,
            _ => null
        };
    }
}

[HarmonyPatch]
internal static class StitchHovertipPatch
{
    [HarmonyPatch(typeof(CardModel), "HoverTips", MethodType.Getter)]
    internal static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        var cardStitch = __instance.GetModifier<StitchCardModifier>();
        if (cardStitch is { StitchedCard: not null } && __instance.IsMutable)
            __result = [.. __result, new CardHoverTip(cardStitch.StitchedCard)];

        return __result;
    }
}

[HarmonyPatch]
internal static class StitchRemovePatch
{
    [HarmonyPatch(typeof(AbstractModel), "AfterCardChangedPiles")]
    internal static async void Postfix(CardModel card, PileType oldPileType, AbstractModel? clonedBy,
        AbstractModel __instance)
    {
        if (card?.Pile == null) return;

        if (card != __instance) return;

        var cardStitch = card.GetModifier<StitchCardModifier>();
        if (cardStitch == null) return;
        if (cardStitch.StitchedCard?.Pile == null ||
            !cardStitch.StitchedCard.IsInCombat)
        {
            if (!TailorStitchQueueSingleton.BeingPlayed.Contains(card)) await StitchCmd.UnstitchCard(card);
        }
        else if (card.Pile.Type == PileType.Exhaust || cardStitch.StitchedCard.Pile.Type == PileType.Exhaust ||
                 card.Pile.Type == PileType.None || cardStitch.StitchedCard.Pile.Type == PileType.None)
        {
            if (!TailorStitchQueueSingleton.BeingPlayed.Contains(cardStitch.StitchedCard))
                await StitchCmd.UnstitchCard(cardStitch.StitchedCard);
            if (!TailorStitchQueueSingleton.BeingPlayed.Contains(card)) await StitchCmd.UnstitchCard(card);
        }
    }
}

public class StitchOverlayAdd
{
    private static readonly string _scenePath = "res://TheTailor/scenes/cards/overlays/stitch.tscn";

    public static AddedNode<NCard, StitchOverlay> StitchOverlay = new(_scenePath, (card, display) =>
    {
        var cardContainer = card.GetChild(0);
        cardContainer.AddChild(display);
        display.Visible = card.Model?.GetModifier<StitchCardModifier>() != null;
    });
}

[HarmonyPatch]
internal static class StitchOverlayPatch
{
    [HarmonyPatch(typeof(NCard), "ReloadOverlay")]
    internal static void Postfix(NCard __instance)
    {
        if (__instance.Model == null) return;

        foreach (var node in __instance.GetChild(0).GetChildren())
            if (node is StitchOverlay stitchOverlay)
            {
                stitchOverlay.Visible = __instance.Model.GetModifier<StitchCardModifier>() != null;
            }
    }
}

[HarmonyPatch]
internal static class RemoveStitchOnCopiesPatch
{
    [HarmonyPatch(typeof(CardModel), "CreateClone")]
    internal static CardModel Postfix(CardModel __result, CardModel __instance)
    {
        var cardStitch = __result.GetModifier<StitchCardModifier>();
        if (cardStitch != null && __instance.IsMutable)
        {
            __result.RemoveKeyword(Keywords.Stitched);
            CardModifier.RemoveModifier(__result, cardStitch);
            NCard.FindOnTable(__result)?.ReloadOverlay();
        }

        return __result;
    }
}