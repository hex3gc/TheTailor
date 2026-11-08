using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class SpareMaterial() : CustomCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/spareMaterialBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/spareMaterialBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/spareMaterialBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Delicate", -999)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.BurlapMinion),
        IsUpgraded
            ? HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate)
            : HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await TailorMinionCmd.AddOrReplaceMinion<MinionBurlap>(choiceContext, Owner, true);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
        DynamicVars["Delicate"].BaseValue = 2;
    }
}