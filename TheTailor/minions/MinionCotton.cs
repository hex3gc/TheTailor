using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Powers;

namespace TheTailor.Minions;

public sealed class MinionCotton : TailorMinion
{
    public override int MinInitialHp => 3;
    public override int MaxInitialHp => 3;

    protected override string VisualsPath => "res://TheTailor/scenes/minions/minionCotton.tscn";

    public override async Task OnSummon(PlayerChoiceContext playerChoiceContext, Player owner,
        MinionSummonOptions options)
    {
        await PowerCmd.Apply<TailorMadePower>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
        await PowerCmd.Apply<CottonPower>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
    }
}