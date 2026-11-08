using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Powers;

namespace TheTailor.Minions;

public sealed class MinionSilk : TailorMinion
{
    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;

    protected override string VisualsPath => "res://TheTailor/scenes/minions/minionSilk.tscn";

    public override async Task OnSummon(PlayerChoiceContext playerChoiceContext, Player owner,
        MinionSummonOptions options)
    {
        await PowerCmd.Apply<TailorMadePower>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
        await PowerCmd.Apply<SilkPower>(playerChoiceContext, Creature, 2m, owner.Creature, options.Source);
    }
}