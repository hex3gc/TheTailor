using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Cards.Token;
using TheTailor.Character;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class SwatchStrike() : CustomCardModel(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/swatchStrikeBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/swatchStrikeBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/swatchStrikeBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Patch>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move), new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await Patch.CreateInHand(Owner, DynamicVars.Cards.IntValue, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}