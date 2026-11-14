using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Ancient;

[Pool(typeof(TheTailorCardPool))]
public class Atelier() : CustomCardModel(9, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/atelierBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/atelierBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/atelierBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AtelierPower>(choiceContext, Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}