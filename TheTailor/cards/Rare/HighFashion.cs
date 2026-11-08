using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class HighFashion() : CustomCardModel(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/highFashionBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/highFashionBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/highFashionBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Upgrades", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<HighFashionPower>(choiceContext, Owner.Creature, DynamicVars["Upgrades"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}