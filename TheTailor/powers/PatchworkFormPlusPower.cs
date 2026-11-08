using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Cards;
using TheTailor.Cards.Token;
using CardSelectorPrefsExtensions = TheTailor.Extensions.CardSelectorPrefsExtensions;

namespace TheTailor.Powers;

public sealed class PatchworkFormPlusPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/patchworkFormSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/patchworkForm.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/patchworkForm.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner == player.Creature)
        {
            Flash();

            for (var i = 0; i < Amount; i++)
            {
                var cards = await Patch.CreateInHand(player, 1, CombatState);
                if (cards == null || cards.Count() <= 0) continue;

                var card = cards.First();

                CardCmd.Upgrade(card);

                card.AddModifier<StitchBlockModifier>();

                var cardModel = await CardSelectCmd.FromHand(
                    prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchPatchSelectionPrompt, 1),
                    context: choiceContext, player: Owner.Player, filter: StitchCmd.CanBeStitchedExcluding,
                    source: this);
                if (cardModel != null && cardModel.Count() == 1)
                    await StitchCmd.StitchCards(card, cardModel.ElementAt(0));

                var stitchBlockModifier = card.GetModifier<StitchBlockModifier>();
                if (stitchBlockModifier != null) CardModifier.RemoveModifier(card, stitchBlockModifier);
            }
        }
    }
}