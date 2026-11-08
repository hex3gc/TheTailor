using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Cards.Token;
using TheTailor.Character;

namespace TheTailor.Cards.Multiplayer.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class DoItYourself() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/doItYourselfBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/doItYourselfBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/doItYourselfBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<SewingKit>(IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (cardPlay.Target != null && cardPlay.Target.IsPlayer)
        {
            var cm = await SewingKit.CreateInHand(cardPlay.Target.Player, CombatState);
            if (IsUpgraded) CardCmd.Upgrade(cm);
        }
    }
}