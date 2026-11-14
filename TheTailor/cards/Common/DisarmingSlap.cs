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
using TheTailor.Powers;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class DisarmingSlap() : CustomCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/disarmingSlap.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/disarmingSlap.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/disarmingSlapBeta.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new("StrengthDown", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await PowerCmd.Apply<DisarmingSlapStrengthPower>(choiceContext, cardPlay.Target,
            DynamicVars["StrengthDown"].BaseValue, Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthDown"].UpgradeValueBy(1);
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}