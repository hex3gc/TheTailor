using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheTailor.Powers;

public sealed class LightTouchPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/lightTouchSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/lightTouch.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/lightTouch.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(Keywords.Delicate)];
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.DynamicVars.ContainsKey("Delicate") &&
            cardPlay.Card.DynamicVars["Delicate"].BaseValue >= 0)
        {
            var internalData = GetInternalData<Data>();
            if (!internalData.thisTurn)
            {
                Flash();
                await PlayerCmd.GainEnergy(Amount, Owner.Player);
                internalData.thisTurn = true;
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            var data = GetInternalData<Data>();
            data.thisTurn = false;
        }
    }

    private class Data
    {
        public bool thisTurn;
    }
}