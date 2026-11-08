using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class StrikeAPose() : CustomCardModel(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override bool HasEnergyCostX => true;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/strikeAPoseBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/strikeAPoseBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/strikeAPoseBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var xCost = ResolveEnergyXValue();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitCount(xCost)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var targets = CombatState.HittableEnemies.ToList();
        await PowerCmd.Apply<StrengthPower>(choiceContext, targets, -xCost, Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}