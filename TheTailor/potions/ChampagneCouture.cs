using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using TheTailor.Character;

namespace TheTailor.Potions;

[Pool(typeof(TheTailorPotionPool))]
public class ChampagneCouture : CustomPotionModel
{
    public override string? CustomPackedImagePath => "res://TheTailor/images/potions/champagneCouture.png";
    public override string? CustomPackedOutlinePath => "res://TheTailor/images/potions/champagneCoutureOutline.png";
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.AnyTime;
    public override TargetType TargetType => TargetType.Self;
    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        IEnumerable<CardModel>? _selection;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };
        _selection = await CardSelectCmd.FromDeckForUpgrade(Owner, prefs);
        if (!_selection.Any()) return;
        foreach (var item in _selection)
        {
            CardCmd.Upgrade(item, CardPreviewStyle.None);
            if (Owner.PlayerCombatState != null)
            {
                var enumerable =
                    Owner.PlayerCombatState.AllCards.Where(cm => cm.IsUpgradable && cm.DeckVersion == item);
                foreach (var cardModel in enumerable) CardCmd.Upgrade(cardModel);
            }
        }
    }
}