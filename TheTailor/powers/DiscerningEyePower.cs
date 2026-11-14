using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheTailor.Powers;

public sealed class DiscerningEyePower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/discerningEyeSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/discerningEye.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/discerningEye.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player == Owner.Player)
        {
            var data = GetInternalData<Data>();
            if (data.playsThisTurn < Amount && cardPlay.Card.Type == CardType.Skill)
            {
                data.playsThisTurn++;
                var cardModel = await CardSelectCmd.FromHandForUpgrade(choiceContext, Owner.Player, this);
                if (cardModel != null) CardCmd.Upgrade(cardModel);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            var data = GetInternalData<Data>();
            data.playsThisTurn = 0;
        }
    }

    private class Data
    {
        public int playsThisTurn;
    }
}