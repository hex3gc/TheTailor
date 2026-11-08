using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Cards.Token;

namespace TheTailor.Powers;

public sealed class InfinitePatchesPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/infinitePatchesSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/infinitePatches.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/infinitePatches.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner == player.Creature)
        {
            Flash();
            IEnumerable<CardModel?> cardModels = await Patch.CreateInHand(player, Amount, CombatState);
        }
    }
}