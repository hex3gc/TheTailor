using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Minions;
using TheTailor.Powers;
using MinionLib.Commands;
using MinionLib.Utilities;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Bolster() : CustomCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/bolsterBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/bolsterBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/bolsterBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Power", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.BurlapMinion)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await TailorMinionCmd.AddOrReplaceMinion<MinionBurlap>(choiceContext, Owner, true);

        var accessor = new PetsOrderAccessor(Owner);
        if (accessor is { Pets.Count: > 0 })
            foreach (var creature in accessor.Pets)
                switch (creature.Monster)
                {
                    case MinionLinen:
                        await PowerCmd.Apply<LinenPower>(choiceContext, creature, DynamicVars["Power"].IntValue,
                            Owner.Creature, this);
                        break;
                    case MinionCotton:
                        await PowerCmd.Apply<CottonPower>(choiceContext, creature, DynamicVars["Power"].IntValue,
                            Owner.Creature, this);
                        break;
                    case MinionDenim:
                        await PowerCmd.Apply<DenimPower>(choiceContext, creature, DynamicVars["Power"].IntValue,
                            Owner.Creature, this);
                        break;
                    case MinionWool:
                        await PowerCmd.Apply<WoolPower>(choiceContext, creature, DynamicVars["Power"].IntValue,
                            Owner.Creature, this);
                        break;
                    case MinionSilk:
                        await PowerCmd.Apply<SilkPower>(choiceContext, creature, DynamicVars["Power"].IntValue,
                            Owner.Creature, this);
                        break;
                    case MinionBurlap:
                        await PowerCmd.Apply<BurlapPower>(choiceContext, creature, DynamicVars["Power"].IntValue,
                            Owner.Creature, this);
                        break;
                }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1);
    }
}