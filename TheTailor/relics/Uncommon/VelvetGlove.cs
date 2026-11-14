using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheTailor.Character;

namespace TheTailor.Relics.Uncommon;

[Pool(typeof(TheTailorRelicPool))]
public class VelvetGlove : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string PackedIconPath => "res://TheTailor/images/relics/velvetGlove.png";
    protected override string PackedIconOutlinePath => "res://TheTailor/images/relics/velvetGloveOutline.png";
    protected override string BigIconPath => "res://TheTailor/images/relics/velvetGloveBig.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Strength", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(Keywords.Delicate), HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner == Owner && card.DynamicVars.ContainsKey("Delicate") &&
            card.DynamicVars["Delicate"].BaseValue >= 0)
        {
            Flash();
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["Strength"].IntValue,
                Owner.Creature, null);
        }
    }
}