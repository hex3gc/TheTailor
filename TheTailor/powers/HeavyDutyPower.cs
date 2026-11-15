using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheTailor.Minions;

namespace TheTailor.Powers;

public sealed class HeavyDutyPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/heavyDutySmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/heavyDuty.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/heavyDuty.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.PetOwner != null && Owner == creature.PetOwner.Creature && creature.Monster is TailorMinion)
        {
            var data = GetInternalData<Data>();
            if (data.deathsOnTurn < Amount)
            {
                Flash();
                await PowerCmd.Apply<StrengthPower>(choiceContext, creature.PetOwner.Creature, 1, creature.PetOwner.Creature, null);
                data.deathsOnTurn++;
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
    IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            var data = GetInternalData<Data>();
            data.deathsOnTurn = 0;
        }
    }


    private class Data
    {
        public int deathsOnTurn;
    }
}