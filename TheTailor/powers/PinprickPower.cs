using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheTailor.Powers;

public sealed class PinprickPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/pinprickSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/pinprick.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/pinprick.png";
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner == target && result.TotalDamage > 0 && result.Props.HasFlag(ValueProp.Move) && dealer != null &&
            dealer.Side == CombatSide.Player)
        {
            if (dealer.PetOwner != null)
                await CreatureCmd.GainBlock(dealer.PetOwner.Creature, new BlockVar(Amount, ValueProp.Unpowered), null);
            else
                await CreatureCmd.GainBlock(dealer, new BlockVar(Amount, ValueProp.Unpowered), null);
        }
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (Owner == creature && Applier != null)
            await CreatureCmd.GainBlock(Applier, new BlockVar(Amount, ValueProp.Unpowered), null);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}