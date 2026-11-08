using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheTailor.Minions;

namespace TheTailor.Powers;

public sealed class BurlapStrengthPower : CustomTemporaryPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/burlapStrengthSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/burlapStrength.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/burlapStrength.png";
    public override AbstractModel OriginModel => ModelDb.Monster<MinionBurlap>();
    public override PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();

    protected override Func<PlayerChoiceContext, Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc =>
        PowerCmd.Apply<StrengthPower>;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
}