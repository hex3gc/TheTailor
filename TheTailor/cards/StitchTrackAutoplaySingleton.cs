using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheTailor;

/// <summary>
///     Used for tracking which enemy is attacking, for a Minion applying debuffs or effects on-death.
/// </summary>
public class StitchTrackAutoplaySingleton() : CustomSingletonModel(HookType.Combat)
{
    public static readonly List<CardModel> BlockedFromAutoplay = [];

    public override async Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        if (type == StitchedAutoPlayType.Stitched) BlockedFromAutoplay.Add(card);
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        BlockedFromAutoplay.Remove(cardPlay.Card);
    }
}