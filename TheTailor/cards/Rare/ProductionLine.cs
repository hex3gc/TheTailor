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

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class ProductionLine() : CustomCardModel(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/productionLineBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/productionLineBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/productionLineBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.LinenMinion)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Minions", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ProductionLinePower>(choiceContext, Owner.Creature, DynamicVars["Minions"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Minions"].UpgradeValueBy(1);
    }
}