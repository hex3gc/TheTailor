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
public class Resourcefulness() : CustomCardModel(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/resourcefulnessBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/resourcefulnessBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/resourcefulnessBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ResourcefulnessPower>(choiceContext, Owner.Creature, DynamicVars.Heal.IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(1);
    }
}