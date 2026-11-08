using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using TheTailor.Cards.Token;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class Inspiration() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/inspirationBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/inspirationBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/inspirationBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new("Sharp", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Patch>(), HoverTipFactory.FromEnchantment<Sharp>(DynamicVars["Sharp"].IntValue).First()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<InspirationPower>(choiceContext, cardPlay.Card.Owner.Creature,
            DynamicVars["Sharp"].BaseValue, cardPlay.Card.Owner.Creature, cardPlay.Card);

        for (var i = 0; i < DynamicVars.Cards.BaseValue; i++)
        {
            CardModel card = CombatState.CreateCard<Patch>(Owner);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Owner, CardPilePosition.Random));
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Sharp"].UpgradeValueBy(2);
    }
}