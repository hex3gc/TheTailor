using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Minions;

namespace TheTailor.Powers;

public sealed class PartingGiftPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/partingGiftSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/partingGift.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/partingGift.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature is { Monster: TailorMinion, IsPet: true } && creature.PetOwner == Owner.Player &&
            TailorMinionCmd.GetMinionCount<TailorMinion>(Owner.Player) <= 0)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}