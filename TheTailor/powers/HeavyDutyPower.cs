using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
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

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.PetOwner != null && Owner == creature.PetOwner.Creature && creature.Monster is TailorMinion)
        {
            Flash();
            await PowerCmd.Apply<StrengthPower>(choiceContext, creature.PetOwner.Creature, Amount,
                creature.PetOwner.Creature, null);
        }
    }
}