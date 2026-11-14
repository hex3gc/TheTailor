using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class Matching() : CustomCardModel(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override int MaxUpgradeLevel => 99999;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/matching.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/matching.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/matchingBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var enumerable = cardPlay.Card.Owner.PlayerCombatState.AllCards.Where(cm => cm is Matching && cm.IsUpgradable);
        foreach (var cardModel in enumerable) CardCmd.Upgrade(cardModel);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}