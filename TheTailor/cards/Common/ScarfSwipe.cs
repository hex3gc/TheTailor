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
public class ScarfSwipe() : CustomCardModel(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/scarfSwipe.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/scarfSwipe.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/scarfSwipeBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}