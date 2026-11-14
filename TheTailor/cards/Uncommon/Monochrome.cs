using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Character;
using TheTailor.Extensions;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class Monochrome() : CustomCardModel(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/monochrome.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/monochrome.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/monochromeBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Stitch)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var cardModels = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchCloneSelectionPrompt, 1),
            context: choiceContext, player: Owner, filter: StitchCmd.CanBeStitched, source: this);
        if (cardModels != null && cardModels.Count() >= 1)
        {
            var cm = cardModels.First();
            if (IsUpgraded && cm.IsUpgradable) CardCmd.Upgrade(cm);

            var cm2 = cm.CreateClone();
            await CardPileCmd.AddGeneratedCardToCombat(cm2, PileType.Hand, Owner, CardPilePosition.Top);
            await StitchCmd.StitchCards(cm, cm2);
        }
    }
}