using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace TheTailor.Cards;

public static class StitchCmd
{
    private static readonly List<CardType> UnstitchableTypes = [CardType.Quest, CardType.None];

    public static async Task StitchCards(CardModel card1, CardModel card2)
    {
        if (UnstitchableTypes.Contains(card1.Type) || UnstitchableTypes.Contains(card2.Type))
        {
            Log.Error("Attempted to stitch 2 unstitchable cards; these should be filtered by StitchCmd.CanBeStitched!");
            return;
        }

        CardModifier.AddModifier<StitchCardModifier>(card1);
        CardModifier.AddModifier<StitchCardModifier>(card2);
        var card1Stitch = card1.GetModifier<StitchCardModifier>();
        var card2Stitch = card2.GetModifier<StitchCardModifier>();
        card1Stitch.StitchedCard = card2;
        card2Stitch.StitchedCard = card1;
        if (card1 is IOnStitchEffect) (card1 as IOnStitchEffect).OnStitch(card1, card2);
        if (card2 is IOnStitchEffect) (card2 as IOnStitchEffect).OnStitch(card2, card1);
        NCard.FindOnTable(card1)?.ReloadOverlay();
        NCard.FindOnTable(card2)?.ReloadOverlay();
    }

    public static async Task UnstitchCard(CardModel card1)
    {
        if (card1 == null || card1.Pile == null) return;

        var cardStitch1 = card1.GetModifier<StitchCardModifier>();
        if (cardStitch1 != null)
        {
            card1.RemoveKeyword(Keywords.Stitched);
            if (card1 is IOnStitchEffect) (card1 as IOnStitchEffect).OnUnstitch(card1);
            CardModifier.RemoveModifier(card1, cardStitch1);
            NCard.FindOnTable(card1)?.ReloadOverlay();
        }
    }

    public static async Task UnstitchRelatedCard(CardModel card1)
    {
        if (card1.Pile == null) return;

        var cardStitch1 = card1.GetModifier<StitchCardModifier>();
        var card2 = cardStitch1?.StitchedCard;
        if (card2 is { Pile: not null })
        {
            var cardStitch2 = card2.GetModifier<StitchCardModifier>();
            if (cardStitch2 != null)
            {
                card2.RemoveKeyword(Keywords.Stitched);
                if (card2 is IOnStitchEffect) (card2 as IOnStitchEffect).OnUnstitch(card2);
                CardModifier.RemoveModifier(card2, cardStitch2);
                NCard.FindOnTable(card2)?.ReloadOverlay();
            }
        }
    }

    public static bool CanBeStitched(CardModel cardModel)
    {
        var ret = cardModel.GetModifier<StitchCardModifier>() == null
                  && !cardModel.Keywords.Contains(CardKeyword.Unplayable)
                  && !UnstitchableTypes.Contains(cardModel.Type)
                  && cardModel.IsMutable;

        return ret;
    }

    public static bool CanBeStitchedExcluding(CardModel cardModel)
    {
        var ret = cardModel.GetModifier<StitchCardModifier>() == null
                  && !cardModel.Keywords.Contains(CardKeyword.Unplayable)
                  && !UnstitchableTypes.Contains(cardModel.Type)
                  && cardModel.IsMutable
                  && cardModel.GetModifier<StitchBlockModifier>() == null;

        return ret;
    }
}