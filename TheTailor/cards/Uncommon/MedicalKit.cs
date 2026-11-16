using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using TheTailor.Cards.Token;
using TheTailor.Character;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class MedicalKit() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Patch>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new("Delicate", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var cardModels = await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, DynamicVars.Cards.IntValue, DynamicVars.Cards.IntValue), context: choiceContext, player: Owner, filter: null, source: this);
        foreach (var cardModel in cardModels)
        {
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
        IEnumerable<CardModel> cardModels1 = await Patch.CreateInHand(Owner, DynamicVars.Cards.IntValue, CombatState);
        if (IsUpgraded)
        {
            foreach (var cardModel in cardModels1)
            {
                CardCmd.Upgrade(cardModel);
            }
        }
    }
}