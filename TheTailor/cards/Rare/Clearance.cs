using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Clearance() : CustomCardModel(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/clearanceBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/clearanceBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/clearanceBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        for (var i = 0; i < 2; i++)
        {
            var card = CreateClone();
            card.EnergyCost.SetThisCombat(EnergyCost.GetResolved() + 1);

            var cardPileAddResult = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, Owner);
            // await StitchCmd.UnstitchCard(cardPileAddResult.cardAdded);
            CardCmd.PreviewCardPileAdd(cardPileAddResult);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }
}