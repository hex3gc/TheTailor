using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheTailor.Powers;

namespace TheTailor.Minions;

public sealed class MinionLeather : TailorMinion
{
    public override int MinInitialHp => 6;
    public override int MaxInitialHp => 6;
    protected override string VisualsPath => "res://TheTailor/scenes/minions/minionLeather.tscn";

    public override async Task OnSummon(PlayerChoiceContext playerChoiceContext, Player owner,
        MinionSummonOptions options)
    {
        await PowerCmd.Apply<TailorMadePower>(playerChoiceContext, Creature, 1m, owner.Creature, options.Source);
        await CreatureCmd.TriggerAnim(Creature, "cast", 0f);
    }
}