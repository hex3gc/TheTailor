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
using TheTailor.Minions;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Coordination() : CustomCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/coordinationBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/coordinationBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/coordinationBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move), new("Hits", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.BurlapMinion),
        HoverTipFactory.FromKeyword(TheTailor.Keywords.DenimMinion)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var strengthTotal = TailorMinionCmd.GetMinionCount<MinionDenim>(Owner) +
                            TailorMinionCmd.GetMinionCount<MinionBurlap>(Owner);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, strengthTotal, Owner.Creature, this);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHitCount(DynamicVars["Hits"].IntValue)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1);
    }
}