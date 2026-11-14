using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheTailor.Powers;

public sealed class WoolWeakPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/woolWeakenSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/woolWeaken.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/woolWeaken.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.HasPower<WoolWeakPower>() && cardPlay.Card.Type == CardType.Attack)
        {
            var targets = cardPlay.Card.TargetType != TargetType.AllEnemies ? 
                [cardPlay.Target] : 
                cardPlay.Card.CombatState.HittableEnemies.ToList();
            await PowerCmd.Apply<WeakPower>(choiceContext, targets, 1m, cardPlay.Card.Owner.Creature, cardPlay.Card);
            await PowerCmd.Decrement(this);
        }
    }
}