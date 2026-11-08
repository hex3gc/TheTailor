using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheTailor.Powers;

public sealed class BurlapPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/burlapSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/burlap.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/burlap.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.IsPet && Owner.PetOwner == player)
        {
            Flash();
            await PowerCmd.Apply<BurlapStrengthPower>(choiceContext, player.Creature, Amount, Owner, null);
        }
    }
}