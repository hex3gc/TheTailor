using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using TheTailor.Minions;

namespace TheTailor.Relics.Ancient;

[Pool(typeof(SharedRelicPool))]
public class MastersToolset : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    public override bool SpawnsPets => true;
    public override string PackedIconPath => "res://TheTailor/images/relics/mastersToolkit.png";
    protected override string PackedIconOutlinePath => "res://TheTailor/images/relics/mastersToolkitOutline.png";
    protected override string BigIconPath => "res://TheTailor/images/relics/mastersToolkitBig.png";

    public override async Task BeforeCombatStart()
    {
        for (var i = 0; i < 2; i++)
        {
            var random = Owner.RunState.Rng.CombatCardGeneration.NextInt(5);

            switch (random)
            {
                case 0:
                    await TailorMinionCmd.AddOrReplaceMinion<MinionLinen>(new ThrowingPlayerChoiceContext(), Owner,
                        true, withTriggers: 1);
                    break;
                case 1:
                    await TailorMinionCmd.AddOrReplaceMinion<MinionDenim>(new ThrowingPlayerChoiceContext(), Owner,
                        true, withTriggers: 1);
                    break;
                case 2:
                    await TailorMinionCmd.AddOrReplaceMinion<MinionSilk>(new ThrowingPlayerChoiceContext(), Owner,
                        true);
                    break;
                case 3:
                    await TailorMinionCmd.AddOrReplaceMinion<MinionBurlap>(new ThrowingPlayerChoiceContext(), Owner,
                        true);
                    break;
                case 4:
                    await TailorMinionCmd.AddOrReplaceMinion<MinionWool>(new ThrowingPlayerChoiceContext(), Owner,
                        true);
                    break;
            }
        }
    }
}