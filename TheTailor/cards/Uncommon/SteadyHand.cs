using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class SteadyHand() : CustomCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/steadyHandBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/steadyHandBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/steadyHandBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new("SteadyHand", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<SteadyHandPower>(choiceContext, new List<Creature> { Owner.Creature },
            DynamicVars["SteadyHand"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SteadyHand"].UpgradeValueBy(1m);
    }
}