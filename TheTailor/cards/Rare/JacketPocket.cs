using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Cards.Token;
using TheTailor.Character;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class JacketPocket() : CustomCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/jacketPocketBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/jacketPocketBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/jacketPocketBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Patch>(),
        IsUpgraded
            ? HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate)
            : HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Delicate", -999), new CardsVar(3)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await Patch.CreateInHand(Owner, DynamicVars.Cards.IntValue, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Delicate"].BaseValue = 2;
        RemoveKeyword(CardKeyword.Exhaust);
    }
}