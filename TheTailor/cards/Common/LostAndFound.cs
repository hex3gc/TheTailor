using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class LostAndFound() : CustomCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/lostAndFoundBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/lostAndFoundBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/lostAndFoundBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Delicate", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheTailor.Keywords.LeatherMinion),
        HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await TailorMinionCmd.AddOrReplaceMinion<MinionLeather>(choiceContext, Owner, true);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player == Owner &&
            CombatManager.Instance.History.CardPlaysFinished.Any(e =>
                e.HappenedLastPlayerTurn(Owner) && e.CardPlay.Card == this))
        {
            var pile = Pile;
            if (pile != null && pile.Type != PileType.Hand && pile.Type != PileType.Exhaust)
                await CardPileCmd.Add(this, PileType.Hand);
        }
    }


    protected override void OnUpgrade()
    {
        DynamicVars["Delicate"].UpgradeValueBy(1);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}