using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Character;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class Trim() : CustomCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/trimBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/trimBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/trimBeta.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var cardModels = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1), context: choiceContext,
            player: Owner, filter: null, source: this);
        foreach (var cardModel in cardModels) await CardCmd.Exhaust(choiceContext, cardModel);

        if (IsUpgraded)
        {
            foreach (var item in PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsUpgradable)) CardCmd.Upgrade(item);
        }
        else
        {
            var cardModel = await CardSelectCmd.FromHandForUpgrade(choiceContext, Owner, this);
            if (cardModel != null) CardCmd.Upgrade(cardModel);
        }
    }
}