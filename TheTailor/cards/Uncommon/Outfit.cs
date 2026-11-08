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
public class Outfit() : CustomCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/outfitBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/outfitBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/outfitBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var minionCount = TailorMinionCmd.GetMinionCount<TailorMinion>(Owner);
        if (minionCount > 0)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitCount(minionCount)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}