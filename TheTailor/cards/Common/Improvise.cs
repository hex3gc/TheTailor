using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Cards.Token;
using TheTailor.Character;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class Improvise() : CustomCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/improviseBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/improviseBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/improviseBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Patch>(IsUpgraded), HoverTipFactory.FromCard<SewingKit>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await SewingKit.CreateInHand(Owner, CombatState);
        var patch = await Patch.CreateInHand(Owner, CombatState);
        if (patch != null && IsUpgraded) CardCmd.Upgrade(patch);
    }
}