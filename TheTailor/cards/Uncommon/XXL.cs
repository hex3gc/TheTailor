using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class XXL() : CustomCardModel(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override int MaxUpgradeLevel => 99999;
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/xxlBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/xxlBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/xxlBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(12)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.LinenMinion),
        HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO : Heavy Duty is a card not a relic i don't know whats the plan here.
        var extraMaxHp = Owner.Relics.OfType<HeavyDuty>().Count();

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await TailorMinionCmd.AddOrReplaceMinion<MinionLinen>(choiceContext, Owner, true,
            DynamicVars.Heal.IntValue + extraMaxHp);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(4);
    }
}