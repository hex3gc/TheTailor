using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheTailor.Powers;

public sealed class CottonPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/cottonSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/cotton.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/cotton.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
            foreach (var creature in participants)
                if (Owner.IsPet && Owner.PetOwner == creature.Player)
                {
                    Flash();
                    await CreatureCmd.Heal(Owner.PetOwner.Creature, Amount);
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner,
                        new DamageVar(Amount, ValueProp.Unpowered | ValueProp.Unblockable), Owner);
                }
    }
}