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
public class BackPocket() : CustomCardModel(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/backPocketBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/backPocketBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/backPocketBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Stitch)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var cardModels = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchSelectionPrompt, 1), context: choiceContext,
            player: Owner, filter: StitchCmd.CanBeStitched, source: this);
        var cardModels2 = await CardSelectCmd.FromCombatPile(
            prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchSelectionPrompt, 1), context: choiceContext,
            player: Owner, filter: StitchCmd.CanBeStitched, pile: PileType.Discard.GetPile(Owner));
        if (cardModels != null && cardModels2 != null && cardModels.Count() == 1 && cardModels2.Count() == 1)
        {
            var cardModel = cardModels.ElementAt(0);
            var cardModel2 = cardModels2.ElementAt(0);
            if (IsUpgraded && cardModel.IsUpgradable) CardCmd.Upgrade(cardModel);
            if (IsUpgraded && cardModel2.IsUpgradable) CardCmd.Upgrade(cardModel2);
            await StitchCmd.StitchCards(cardModel, cardModel2);
        }
    }
}