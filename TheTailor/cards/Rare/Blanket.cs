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
public class Blanket() : CustomCardModel(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/blanketBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/blanketBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/blanketBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.WoolMinion)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        for (var i = 0; i < 2; i++) await TailorMinionCmd.AddOrReplaceMinion<MinionWool>(choiceContext, Owner, true);

        await TailorMinionCmd.GiveMinionHealth<TailorMinion>(choiceContext, cardPlay.Card.Owner,
            DynamicVars.Heal.IntValue, TailorMinionCmd.MinionTriggerType.All);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(1);
    }
}