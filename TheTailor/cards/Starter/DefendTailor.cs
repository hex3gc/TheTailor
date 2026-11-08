using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Starter;

[Pool(typeof(TheTailorCardPool))]
public class DefendTailor() : CustomCardModel(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/defendTailor.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/defendTailor.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/defendTailorBeta.png";
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}