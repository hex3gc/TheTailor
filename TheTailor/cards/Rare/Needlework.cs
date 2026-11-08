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
public class Needlework() : CustomCardModel(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/needleworkBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/needleworkBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/needleworkBeta.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4, ValueProp.Move), new CardsVar(1), new("Delicate", 3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var cardModels = await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        foreach (var cardModel in cardModels)
            if (cardModel.IsUpgradable)
                CardCmd.Upgrade(cardModel);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}