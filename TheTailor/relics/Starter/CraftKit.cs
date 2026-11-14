using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Character;
using TheTailor.Minions;
using TheTailor.Relics.Ancient;

namespace TheTailor.Relics.Starter;

[Pool(typeof(TheTailorRelicPool))]
public class CraftKit : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(Keywords.LeatherMinion)];
    public override bool SpawnsPets => true;
    public override string PackedIconPath => "res://TheTailor/images/relics/craftKit.png";
    protected override string PackedIconOutlinePath => "res://TheTailor/images/relics/craftKitOutline.png";
    protected override string BigIconPath => "res://TheTailor/images/relics/craftKitBig.png";

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<MastersToolset>();
    }

    public override async Task BeforeCombatStart()
    {
        await TailorMinionCmd.AddMinion<MinionLeather>(new ThrowingPlayerChoiceContext(), Owner);
    }
}