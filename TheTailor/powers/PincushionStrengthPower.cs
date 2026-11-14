using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheTailor.Cards.Uncommon;

namespace TheTailor.Powers;

public sealed class PincushionStrengthPower : CustomTemporaryPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/pincushionSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/pincushion.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/pincushion.png";
    public override AbstractModel OriginModel => ModelDb.Card<Pincushion>();
    public override PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();

    protected override Func<PlayerChoiceContext, Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc =>
        PowerCmd.Apply<StrengthPower>;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
}