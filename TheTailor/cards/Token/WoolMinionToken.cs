using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace TheTailor.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class WoolMinionToken() : CustomCardModel(-1, CardType.Status, CardRarity.Token, TargetType.None)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/woolMinionTokenBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/woolMinionTokenBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/woolMinionTokenBeta.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new("ChoiceIndex", -1), new("Health", 0), new("HealthPluralize", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
    }
}