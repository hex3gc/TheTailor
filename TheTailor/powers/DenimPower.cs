using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheTailor.Powers;

/// <summary>
///     Applies strength to the owner when hit
///     Also applies strength on death because AfterDamageReceived doesn't work properly with killing blows on minions
/// </summary>
public sealed class DenimPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/denimSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/denim.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/denim.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != null && Owner == target && result.UnblockedDamage > 0)
        {
            Flash();
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.PetOwner.Creature, Amount, Owner, null);
        }
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (Owner == creature && !wasRemovalPrevented)
        {
            Flash();
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.PetOwner.Creature, Amount, Owner, null);
        }
    }
}