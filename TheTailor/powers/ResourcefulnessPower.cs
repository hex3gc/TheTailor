using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Minions;

namespace TheTailor.Powers;

public sealed class ResourcefulnessPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/resourcefulnessSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/resourcefulness.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/resourcefulness.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner.Creature == Owner && Owner.IsPlayer)
        {
            Flash();
            await TailorMinionCmd.GiveMinionHealth<TailorMinion>(choiceContext, Owner.Player, Amount,
                TailorMinionCmd.MinionTriggerType.Random);
        }
    }

    /*
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, player, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), null, this))
        {
            await CardCmd.Exhaust(choiceContext, item);
        }
    }
    */
}