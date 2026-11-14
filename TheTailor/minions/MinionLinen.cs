using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Powers;

namespace TheTailor.Minions;

public sealed class MinionLinen : TailorMinion
{
    public override int MinInitialHp => 5;
    public override int MaxInitialHp => 5;

    protected override string VisualsPath => "res://TheTailor/scenes/minions/minionLinen.tscn";

    public override async Task OnSummon(PlayerChoiceContext playerChoiceContext, Player owner,
        MinionSummonOptions options)
    {
        await PowerCmd.Apply<TailorMadePower>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
        await PowerCmd.Apply<LinenPower>(playerChoiceContext, Creature, 2m, owner.Creature, options.Source);
        await CreatureCmd.TriggerAnim(Creature, "cast", 0f);
    }
}