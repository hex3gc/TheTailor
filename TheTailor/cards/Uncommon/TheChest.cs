using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Character;

namespace TheTailor.Cards.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class TheChest() : CustomCardModel(9, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/theChestBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/theChestBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/theChestBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card == this && card.Owner == Owner)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

            foreach (var item in PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsUpgradable)) CardCmd.Upgrade(item);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}