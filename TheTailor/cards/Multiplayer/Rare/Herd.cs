using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Multiplayer.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Herd() : CustomCardModel(9, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/herdBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/herdBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/herdBeta.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.LeatherMinion)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (CombatState == null) return;
        List<Creature> list = (from c in CombatState.GetTeammatesOf(Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            for (var j = 0; j < 2; j++) await TailorMinionCmd.AddMinion<MinionLeather>(choiceContext, list[i].Player);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-6);
    }
}