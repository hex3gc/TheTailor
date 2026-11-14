using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class GetCreative() : CustomCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/getCreativeBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/getCreativeBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/getCreativeBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new("Triggers", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var random = Owner.RunState.Rng.CombatCardGeneration.NextInt(5);

        switch (random)
        {
            case 0:
                await TailorMinionCmd.AddOrReplaceMinion<MinionLinen>(choiceContext, Owner, true,
                    withTriggers: DynamicVars["Triggers"].IntValue);
                break;
            case 1:
                await TailorMinionCmd.AddOrReplaceMinion<MinionDenim>(choiceContext, Owner, true,
                    withTriggers: DynamicVars["Triggers"].IntValue);
                break;
            case 2:
                await TailorMinionCmd.AddOrReplaceMinion<MinionSilk>(choiceContext, Owner, true,
                    withTriggers: DynamicVars["Triggers"].IntValue);
                break;
            case 3:
                await TailorMinionCmd.AddOrReplaceMinion<MinionBurlap>(choiceContext, Owner, true,
                    withTriggers: DynamicVars["Triggers"].IntValue);
                break;
            case 4:
                await TailorMinionCmd.AddOrReplaceMinion<MinionWool>(choiceContext, Owner, true,
                    withTriggers: DynamicVars["Triggers"].IntValue);
                break;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["Triggers"].UpgradeValueBy(1);
    }
}