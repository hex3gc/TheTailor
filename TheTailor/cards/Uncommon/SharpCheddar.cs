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

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class SharpCheddar() : CustomCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/sharpCheddarBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/sharpCheddarBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/sharpCheddarBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Move), new("Vigor", 4)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VigorPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["Vigor"].IntValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["Vigor"].UpgradeValueBy(2);
    }
}