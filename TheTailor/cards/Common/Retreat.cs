using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class Retreat() : CustomCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/retreatBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/retreatBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/retreatBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var plays = 1;
        if (TailorMinionCmd.GetMinionCount<TailorMinion>(Owner) <= 0) plays += 2;

        for (var i = 0; i < plays; i++) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}