using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Character;

namespace TheTailor.Cards.Multiplayer.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class BoxOfChocolates() : CustomCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/boxOfChocolatesBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/boxOfChocolatesBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/boxOfChocolatesBeta.png";
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null && cardPlay.Target.IsPlayer)
            foreach (var item in PileType.Hand.GetPile(cardPlay.Target.Player).Cards.Where(c => c.IsUpgradable))
                CardCmd.Upgrade(item);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}