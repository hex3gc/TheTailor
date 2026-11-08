using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheTailor.Extensions;

namespace TheTailor.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class SewingKit() : CustomCardModel(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/sewingKitBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/sewingKitBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/sewingKitBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Stitch)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cardModel = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchSelectionPrompt, 2), context: choiceContext,
            player: Owner, filter: StitchCmd.CanBeStitched, source: this);
        if (cardModel != null && cardModel.Count() == 2)
            await StitchCmd.StitchCards(cardModel.ElementAt(0), cardModel.ElementAt(1));
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public static async Task<CardModel?> CreateInHand(Player owner, ICombatState combatState)
    {
        return (await CreateInHand(owner, 1, combatState)).FirstOrDefault();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(Player owner, int count, ICombatState combatState)
    {
        if (count == 0) return Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding) return Array.Empty<CardModel>();
        var shivs = new List<CardModel>();
        for (var i = 0; i < count; i++) shivs.Add(combatState.CreateCard<SewingKit>(owner));
        await CardPileCmd.AddGeneratedCardsToCombat(shivs, PileType.Hand, owner);
        return shivs;
    }
}