using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Cards.Token;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class PatchworkForm() : CustomCardModel(9, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/patchworkFormBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/patchworkFormBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/patchworkFormBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.Stitch), HoverTipFactory.FromCard<Patch>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<PatchworkFormPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-6);
    }
}