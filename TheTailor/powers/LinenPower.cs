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
///     Applies Vulnerable to the attacker when hit
///     Also applies Vulnerable to the last attacker on death, since AfterDamageReceived doesn't work properly when minions
///     die
/// </summary>
public sealed class LinenPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/linenSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/linen.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/linen.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != null && Owner == target && result.UnblockedDamage > 0)
        {
            Flash();
            await PowerCmd.Apply<VulnerablePower>(choiceContext, dealer, Amount, Owner, null);
        }
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (Owner == creature && !wasRemovalPrevented && TailorLastAttackSingleton.lastAttacker != null)
        {
            Flash();
            await PowerCmd.Apply<VulnerablePower>(choiceContext, TailorLastAttackSingleton.lastAttacker, Amount, Owner,
                null);
        }
    }
}