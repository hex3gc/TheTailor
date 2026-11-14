using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace TheTailor.Powers;

public sealed class ThriftingPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/thriftingSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/thrifting.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/thrifting.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner.Creature == Owner)
        {
            var data = GetInternalData<Data>();
            if (data.exhaustsThisTurn < Amount)
            {
                Flash();
                if (causedByEthereal)
                    GetInternalData<Data>().etherealCount++;
                else
                    await CardPileCmd.Draw(choiceContext, 1m, Owner.Player);
                data.exhaustsThisTurn++;
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            var data = GetInternalData<Data>();
            await CardPileCmd.Draw(choiceContext, data.etherealCount, Owner.Player);
            data.etherealCount = 0;
            data.exhaustsThisTurn = 0;
        }
    }

    private class Data
    {
        public int etherealCount; // Handles Ethereal draw-after-turn
        public int exhaustsThisTurn; // Handles overall exhaust
    }
}