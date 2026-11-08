using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Cards.Token;
using TheTailor.Character;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class MedicalKit() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Patch>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await Patch.CreateInHand(Owner, DynamicVars.Cards.IntValue, CombatState);

        var list = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != null && c.IsTransformable && c.Type == CardType.Status).ToList();
        var list2 = new List<CardTransformation>();
        foreach (var item in list)
        {
            CardModel cardModel = CombatState.CreateCard<Patch>(Owner);
            list2.Add(new CardTransformation(item, cardModel));
        }

        await CardCmd.Transform(list2, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}