using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheTailor.Cards.Token;
using TheTailor.Character;
using TheTailor.Powers;

namespace TheTailor.Cards.Multiplayer.Rare;

[Pool(typeof(TheTailorCardPool))]
public class InfinitePatches() : CustomCardModel(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/infinitePatchesBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/infinitePatchesBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/infinitePatchesBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Patch>(IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<Creature> enumerable = from c in CombatState.GetTeammatesOf(Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
        foreach (var item in enumerable)
            if (IsUpgraded)
                await PowerCmd.Apply<InfinitePatchesPlusPower>(choiceContext, item, 1, Owner.Creature, this);
            else
                await PowerCmd.Apply<InfinitePatchesPower>(choiceContext, item, 1, Owner.Creature, this);
    }
}