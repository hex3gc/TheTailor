using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Extensions;

namespace TheTailor.Cards.Ancient;

[Pool(typeof(TheTailorCardPool))]
public class Weave() : CustomCardModel(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/weaveBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/weaveBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/weaveBeta.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, ValueProp.Move), new("Delicate", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Stitch)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var cardModel = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchSelectionPrompt, 2), context: choiceContext,
            player: Owner, filter: StitchCmd.CanBeStitched, source: this);
        if (cardModel != null && cardModel.Count() == 2)
            await StitchCmd.StitchCards(cardModel.ElementAt(0), cardModel.ElementAt(1));
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Delicate"].UpgradeValueBy(1);
        RemoveKeyword(CardKeyword.Exhaust);
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}