using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Minions;

namespace TheTailor.Powers;

public sealed class PowderKegPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://TheTailor/images/powers/powderKegSmall.png";
    public override string? CustomBigIconPath => "res://TheTailor/images/powers/powderKeg.png";
    public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/powderKeg.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(Keywords.LeatherMinion), HoverTipFactory.FromKeyword(Keywords.BurlapMinion)
    ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.PetOwner != null && Owner == creature.PetOwner.Creature &&
            creature.Monster is MinionLeather or MinionBurlap)
        {
            Flash();
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            foreach (var hittableEnemy in CombatState.HittableEnemies)
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(hittableEnemy));
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies,
                new DamageVar(Amount, ValueProp.Unpowered), Owner);
        }
    }
}