using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Cards;
using TheTailor.Character;
using TheTailor.Extensions;

namespace TheTailor.Relics.Rare;

[Pool(typeof(TheTailorRelicPool))]
public class LivingNeedle : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override string PackedIconPath => "res://TheTailor/images/relics/livingNeedle.png";
    protected override string PackedIconOutlinePath => "res://TheTailor/images/relics/livingNeedleOutline.png";
    protected override string BigIconPath => "res://TheTailor/images/relics/livingNeedleBig.png";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner && Owner.PlayerCombatState.TurnNumber == 1)
        {
            var cardModel = await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchSelectionPrompt, 2),
                context: choiceContext, player: Owner, filter: StitchCmd.CanBeStitched, source: this);
            if (cardModel != null && cardModel.Count() == 2)
            {
                Flash();
                await StitchCmd.StitchCards(cardModel.ElementAt(0), cardModel.ElementAt(1));
            }
        }
    }
}