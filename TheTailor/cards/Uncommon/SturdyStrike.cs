using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class SturdyStrike() : CustomCardModel(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike, CardTag.Minion];
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/sturdyStrikeBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/sturdyStrikeBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/sturdyStrikeBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.DenimMinion)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await TailorMinionCmd.AddOrReplaceMinion<MinionDenim>(choiceContext, Owner, true);
        EnergyCost.UpgradeBy(1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}