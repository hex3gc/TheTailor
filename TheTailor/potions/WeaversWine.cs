using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Cards;
using TheTailor.Character;
using TheTailor.Extensions;

namespace TheTailor.Potions;

[Pool(typeof(TheTailorPotionPool))]
public class WeaversWine : CustomPotionModel
{
    public override string? CustomPackedImagePath => "res://TheTailor/images/potions/weaversWine.png";
    public override string? CustomPackedOutlinePath => "res://TheTailor/images/potions/weaversWineOutline.png";
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(Keywords.Stitch)
    ];

    public override bool PassesCustomUsabilityCheck => IsUsable();

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchSelectionPrompt, 2), context: choiceContext,
            player: Owner, filter: StitchCmd.CanBeStitched, source: this)).ToList();
        if (cardModel.Count == 2)
            await StitchCmd.StitchCards(cardModel.ElementAt(0), cardModel.ElementAt(1));
    }

    private bool IsUsable()
    {
        if (!(Owner.PlayerCombatState?.Hand.Cards.Count >= 2)) return false;
        var playableCards = Owner.PlayerCombatState.Hand.Cards.Count(StitchCmd.CanBeStitched);
        return playableCards >= 2;
    }
}