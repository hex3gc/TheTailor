using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MinionLib.Minion;
using MinionLib.Powers;
using TheTailor.Powers;

namespace TheTailor.Minions;

public sealed class MinionDenim : TailorMinion
{
    public override int MinInitialHp => 7;
    public override int MaxInitialHp => 7;

    protected override string VisualsPath => "res://TheTailor/scenes/minions/minionDenim.tscn";

    public override async Task OnSummon(PlayerChoiceContext playerChoiceContext, Player owner,
        MinionSummonOptions options)
    {
        await PowerCmd.Apply<TailorMadePower>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
        await PowerCmd.Apply<DenimPower>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
        await PowerCmd.Apply<TailorMinionOrderAction>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
    }
}