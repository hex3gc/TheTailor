using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Character;
using TheTailor.Minions;
using MinionLib.Commands;
using MinionLib.Utilities;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class GreatExpectations() : CustomCardModel(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/greatExpectationsBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/greatExpectationsBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/greatExpectationsBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.LeatherMinion),
        HoverTipFactory.FromKeyword(TheTailor.Keywords.DenimMinion), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        List<Creature> petsToKill = new();

        var accessor = new PetsOrderAccessor(Owner);
        if (accessor != null && accessor.Pets != null)
            for (var i = 0; i < accessor.Pets.Count; i++)
                if (accessor.Pets[i].Monster is MinionLeather)
                    petsToKill.Add(accessor.Pets[i]);

        var replaceAmount = petsToKill.Count;

        foreach (var creature in petsToKill)
        {
            creature.RemoveAllPowersInternalExcept();
            await CreatureCmd.Kill(creature, true);
        }

        for (var i = 0; i < replaceAmount; i++) await TailorMinionCmd.AddMinion<MinionDenim>(choiceContext, Owner);

        _ = MinionAnimCmd.Rearrange(duration: 0.5f);
        accessor.SetManualRearranged();
        PetOrderSnapshotManager.TakeSnapshot(Owner);

        await TailorMinionCmd.PutOstyAtBack(choiceContext, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}