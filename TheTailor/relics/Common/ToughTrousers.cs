using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Relics.Common;

[Pool(typeof(TheTailorRelicPool))]
public class ToughTrousers : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override string PackedIconPath => "res://TheTailor/images/relics/toughTrousers.png";
    protected override string PackedIconOutlinePath => "res://TheTailor/images/relics/toughTrousersOutline.png";
    protected override string BigIconPath => "res://TheTailor/images/relics/toughTrousersBig.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(1)];

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature is { Monster: TailorMinion, IsPet: true } && creature.PetOwner == Owner)
        {
            Flash();
            await CreatureCmd.GainMaxHp(creature, DynamicVars.Heal.IntValue);
        }
    }
}