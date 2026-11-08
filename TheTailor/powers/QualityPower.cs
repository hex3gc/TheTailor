using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace TheTailor.Powers;

public sealed class QualityPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/qualitySmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/quality.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/quality.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay is { Card.IsUpgraded: true })
        {
            Flash();
            for (var i = 0; i < Amount; i++)
            {
                var pile = PileType.Hand.GetPile(Owner.Player);
                var cardModel = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(pile.Cards.Where(cm =>
                    cm.MaxUpgradeLevel > cm.CurrentUpgradeLevel && cm.Type != CardType.Status &&
                    cm.Type != CardType.Curse));
                if (cardModel != null) CardCmd.Upgrade(cardModel);
            }
        }

        return Task.CompletedTask;
    }
}