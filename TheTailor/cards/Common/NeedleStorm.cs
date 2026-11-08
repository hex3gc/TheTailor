using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class NeedleStorm() : CustomCardModel(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/needleStorm.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/needleStorm.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/needleStormBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash", null, "dagger_throw.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}