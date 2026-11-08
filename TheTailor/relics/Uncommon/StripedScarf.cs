using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Relics.Uncommon;

[Pool(typeof(TheTailorRelicPool))]
public class StripedScarf : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string PackedIconPath => "res://TheTailor/images/relics/stripedScarf.png";
    protected override string PackedIconOutlinePath => "res://TheTailor/images/relics/stripedScarfOutline.png";
    protected override string BigIconPath => "res://TheTailor/images/relics/stripedScarfBig.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Unpowered)];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Creature.Side)
        {
            var combatState = Owner.Creature.CombatState;
            if (Owner.PlayerCombatState.TurnNumber == 1 && TailorMinionCmd.GetMinionCount<TailorMinion>(Owner) > 0)
            {
                Flash();
                VfxCmd.PlayOnCreatureCenters(combatState.HittableEnemies, "vfx/vfx_attack_slash");
                await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies,
                    new DamageVar(DynamicVars.Damage.IntValue * TailorMinionCmd.GetMinionCount<TailorMinion>(Owner),
                        ValueProp.Unpowered), Owner.Creature);
            }
        }
    }
}