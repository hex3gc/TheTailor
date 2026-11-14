using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Cards.Common;
using TheTailor.Cards.Uncommon;
using TheTailor.Character;

namespace TheTailor.Cards.Multiplayer.Uncommon;

[Pool(typeof(TheTailorCardPool))]
public class SlapForAll() : CustomCardModel(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override int MaxUpgradeLevel => 99999;
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/slapForAllBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/slapForAllBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/slapForAllBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new("Slaps", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SingleSlap>(), HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .Targeting(cardPlay.Target)
            .FromCard(this, cardPlay)
            .WithHitCount(DynamicVars["Slaps"].IntValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        IEnumerable<Creature> enumerable = from c in CombatState.GetTeammatesOf(Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer && c != Owner.Creature
            select c;
        foreach (var teammate in enumerable)
        {
            CardModel card = CombatState.CreateCard<SingleSlap>(teammate.Player);
            card.AddKeyword(CardKeyword.Ethereal);
            await CardPileCmd.AddGeneratedCardsToCombat([card], PileType.Hand, teammate.Player);
            await Cmd.Wait(0.1f);

            foreach (var item in PileType.Hand.GetPile(teammate.Player).Cards.Where(c =>
                         c.IsUpgradable && (c is SingleSlap || c is UltimateSlap || c is SlapForAll ||
                                            c is DisarmingSlap))) CardCmd.Upgrade(item);
        }

        CardCmd.Upgrade(this);

        foreach (var item in PileType.Hand.GetPile(Owner).Cards.Where(c =>
                     c.IsUpgradable && (c is SingleSlap || c is UltimateSlap || c is SlapForAll || c is DisarmingSlap)))
            CardCmd.Upgrade(item);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Slaps"].UpgradeValueBy(1);
    }
}