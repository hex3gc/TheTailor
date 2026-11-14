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

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Luxury() : CustomCardModel(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override int MaxUpgradeLevel => 99999;
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/luxuryBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/luxuryBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/luxuryBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move), new("Delicate", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.SilkMinion),
        HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await TailorMinionCmd.AddOrReplaceMinion<MinionSilk>(choiceContext, Owner, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}