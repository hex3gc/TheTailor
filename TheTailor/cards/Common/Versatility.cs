using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class Versatility() : CustomCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Minion];
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/versatility.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/versatility.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/versatilityBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.LeatherMinion),
        HoverTipFactory.FromKeyword(TheTailor.Keywords.LinenMinion)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var accessor = new PetsOrderAccessor(cardPlay.Card.Owner);
        if (accessor is { Pets: not null })
            for (var i = 0; i < accessor.Pets.Count; i++)
                if (accessor.Pets[i] != null && accessor.Pets[i].Monster is MinionLeather)
                    await TailorMinionCmd.ReplaceMinion<MinionLinen>(choiceContext, cardPlay.Card.Owner, i);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}