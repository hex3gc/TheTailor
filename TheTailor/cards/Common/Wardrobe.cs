using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class Wardrobe() : CustomCardModel(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/wardrobeBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/wardrobeBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/wardrobeBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        foreach (var item in PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsUpgradable)) CardCmd.Upgrade(item);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}