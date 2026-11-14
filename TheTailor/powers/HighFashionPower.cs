using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheTailor.Powers;

public sealed class HighFashionPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/highFashionSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/highFashion.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/highFashion.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(Keywords.Premium)];
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner == player.Creature)
        {
            Flash();
            var enumerable = Owner.Player.PlayerCombatState.AllCards.Where(cm =>
                cm.MaxUpgradeLevel > 1 && cm.IsUpgradable && cm.Type != CardType.Status && cm.Type != CardType.Curse);
            foreach (var cm in enumerable)
                for (var i = 0; i < Amount; i++)
                    CardCmd.Upgrade(cm);
        }
    }
}