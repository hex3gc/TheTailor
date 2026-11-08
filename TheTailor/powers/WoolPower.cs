using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheTailor.Powers;

public sealed class WoolPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/woolSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/wool.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/wool.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WoolWeakPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.IsPet && Owner.PetOwner == player)
        {
            Flash();
            await PowerCmd.Apply<WoolWeakPower>(choiceContext, player.Creature, Amount, Owner, null);
        }
    }

    /*
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            foreach (Creature creature in participants)
            {
                if (Owner.IsPet && Owner.PetOwner == creature.Player && creature.HasPower<WoolWeakPower>())
                {
                    await PowerCmd.Remove<WoolWeakPower>(creature);
                }
            }
        }
    }
    */
}