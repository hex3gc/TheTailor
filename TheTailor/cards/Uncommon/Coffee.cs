using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class Coffee() : CustomCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/coffeeBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/coffeeBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/coffeeBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Delicate", 2), new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        DynamicVars.Energy.UpgradeValueBy(1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Delicate"].UpgradeValueBy(1);
    }
}