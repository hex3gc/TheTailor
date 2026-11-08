using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class DiscerningEye() : CustomCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/discerningEyeBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/discerningEyeBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/discerningEyeBeta.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<DiscerningEyePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}