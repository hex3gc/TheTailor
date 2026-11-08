using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class Finery() : CustomCardModel(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/fineryBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/fineryBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/fineryBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("FineryBlock", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<FineryPower>(choiceContext, Owner.Creature, DynamicVars["FineryBlock"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // DynamicVars.Block.UpgradeValueBy(1m);
        EnergyCost.UpgradeBy(-1);
    }
}