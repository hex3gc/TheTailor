using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheTailor.Powers;

public sealed class FineryPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/finerySmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/finery.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/finery.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay != null && cardPlay.Card.IsUpgraded)
        {
            Flash();
            await CreatureCmd.GainBlock(cardPlay.Card.Owner.Creature, Amount, ValueProp.Unpowered, null);
        }
    }
}