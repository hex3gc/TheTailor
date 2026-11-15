using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheTailor.Cards.Common;
using TheTailor.Cards.Rare;

namespace TheTailor.Powers;

public sealed class StrikeAPoseStrengthPower : CustomTemporaryPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/strikeAPoseStrengthSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/strikeAPoseStrength.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/strikeAPoseStrength.png";
    public override AbstractModel OriginModel => ModelDb.Card<StrikeAPose>();
    public override PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();
    protected override Func<PlayerChoiceContext, Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc => PowerCmd.Apply<StrengthPower>;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    protected override bool InvertInternalPowerAmount => true;
    public override PowerType Type => PowerType.Debuff;
}