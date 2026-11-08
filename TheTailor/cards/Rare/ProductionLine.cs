using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Character;
using TheTailor.Minions;
using TheTailor.Powers;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class ProductionLine() : CustomCardModel(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/productionLineBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/productionLineBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/productionLineBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.LinenMinion)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ProductionLinePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        if (IsUpgraded) await TailorMinionCmd.AddOrReplaceMinion<MinionLinen>(choiceContext, Owner, true);
    }
}