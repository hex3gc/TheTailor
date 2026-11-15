using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheTailor.Minions;

namespace TheTailor.Powers;

public sealed class BolsterPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/bolsterSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/bolster.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/bolster.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.IsPet && Owner.PetOwner == player)
        {
            Flash();
            var accessor = new PetsOrderAccessor(Owner.PetOwner);
            if (accessor is { Pets.Count: > 0 })
            {
                foreach (var creature in accessor.Pets)
                {
                    switch (creature.Monster)
                    {
                        case MinionLinen:
                            await PowerCmd.Apply<LinenPower>(choiceContext, creature, Amount, Owner, null);
                            break;
                        case MinionCotton:
                            await PowerCmd.Apply<CottonPower>(choiceContext, creature, Amount, Owner, null);
                            break;
                        case MinionDenim:
                            await PowerCmd.Apply<DenimPower>(choiceContext, creature, Amount, Owner, null);
                            break;
                        case MinionWool:
                            await PowerCmd.Apply<WoolPower>(choiceContext, creature, Amount, Owner, null);
                            break;
                        case MinionSilk:
                            await PowerCmd.Apply<SilkPower>(choiceContext, creature, Amount, Owner, null);
                            break;
                        case MinionBurlap:
                            await PowerCmd.Apply<BurlapPower>(choiceContext, creature, Amount, Owner, null);
                            break;
                    }
                }
            }
        }
    }
}