using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Potions;

[Pool(typeof(TheTailorPotionPool))]
public class CraftsmansBrew : CustomPotionModel
{
    public override string? CustomPackedImagePath => "res://TheTailor/images/potions/craftsmansBrew.png";
    public override string? CustomPackedOutlinePath => "res://TheTailor/images/potions/craftsmansBrewOutline.png";
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(Keywords.LeatherMinion)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        for (var i = 0; i < 3; i++)
            if (TailorMinionCmd.CanMinionBeAdded(target.Player))
                await TailorMinionCmd.AddMinion<MinionLeather>(choiceContext, Owner);
    }
}