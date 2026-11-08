using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MinionLib.Action;
using MinionLib.Commands;
using MinionLib.Utilities;
using TheTailor.Minions;

#pragma warning disable STS003
public sealed class TailorMinionOrderAction : ActionModel
{
    public override TargetType TargetType => TargetType.None;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => false;

    protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner?.PetOwner == null) return;

        var accessor = new PetsOrderAccessor(Owner.PetOwner);
        if (accessor is { Pets: not null } && accessor.Pets.Contains(Owner) && accessor.Pets.Count > 1)
        {
            accessor.Pets.Remove(Owner);
            accessor.Pets.Insert(0, Owner);
            _ = MinionAnimCmd.Rearrange(duration: 0.5f);
            await CreatureCmd.TriggerAnim(Owner, "cast", 0f);
            accessor.SetManualRearranged();
            PetOrderSnapshotManager.TakeSnapshot(Owner.PetOwner);

            await TailorMinionCmd.PutOstyAtBack(choiceContext, Owner.PetOwner);
        }
    }
}
