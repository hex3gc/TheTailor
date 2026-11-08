using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class Refined() : CustomCardModel(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/refinedBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/refinedBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/refinedBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move), new EnergyVar(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await PowerCmd.Apply<RefinedPower>(choiceContext, cardPlay.Card.Owner.Creature, 1, cardPlay.Card.Owner.Creature,
            cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }
}