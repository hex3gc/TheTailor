using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Minions;
using TheTailor.Powers;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class DressUp() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/dressUpBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/dressUpBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/dressUpBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("TempDex", 2), new("Delicate", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.SilkMinion)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SilkDexterityPower>(choiceContext, Owner.Creature, DynamicVars["TempDex"].IntValue,
            Owner.Creature, this);
        await TailorMinionCmd.AddOrReplaceMinion<MinionSilk>(choiceContext, Owner, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["TempDex"].UpgradeValueBy(2);
    }
}