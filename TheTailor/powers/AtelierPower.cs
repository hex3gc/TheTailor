using BaseLib.Abstracts;
using BaseLib.Common.Rewards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace TheTailor.Powers;

public sealed class AtelierPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/atelierSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/atelier.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/atelier.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        for (var i = 0; i < Amount; i++) room.AddExtraReward(Owner.Player, new RandomCardUpgradeReward(Owner.Player));
        return Task.CompletedTask;
    }
}