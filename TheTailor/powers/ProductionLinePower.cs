using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Minions;

namespace TheTailor.Powers;

public sealed class ProductionLinePower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/productionLineSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/productionLine.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/productionLine.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(Keywords.LinenMinion)];

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner == player.Creature)
            for (var i = 0; i < Amount; i++)
                if (await TailorMinionCmd.AddMinion<MinionLinen>(choiceContext, player, toFront: true))
                    Flash();
                else
                    break;
    }
}