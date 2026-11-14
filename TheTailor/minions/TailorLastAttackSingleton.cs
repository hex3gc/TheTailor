using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;

/// <summary>
///     Used for tracking which enemy is attacking, for a Minion applying debuffs or effects on-death.
/// </summary>
public class TailorLastAttackSingleton() : CustomSingletonModel(HookType.Combat)
{
    public static Creature? lastAttacker;

    public override async Task BeforeAttack(AttackCommand command)
    {
        lastAttacker = command.Attacker;
    }
}