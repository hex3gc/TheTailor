using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Character;
using TheTailor.Potions;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Sommelier() : CustomCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/sommelierBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/sommelierBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/sommelierBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPotion<WeaversWine>()];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PotionCmd.TryToProcure(ModelDb.Potion<WeaversWine>().ToMutable(), Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}