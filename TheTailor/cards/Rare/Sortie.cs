using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;
using TheTailor.Minions;
using MinionLib.Commands;
using MinionLib.Utilities;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Sortie() : CustomCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/sortieBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/sortieBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/sortieBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(22, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<Creature> petsToKill = [];

        var accessor = new PetsOrderAccessor(Owner);
        if (accessor is { Pets: not null })
            foreach (var t in accessor.Pets)
                if (t.Monster is TailorMinion)
                    petsToKill.Add(t);

        foreach (var creature in petsToKill)
        {
            creature.RemoveAllPowersInternalExcept();
            await CreatureCmd.Kill(creature, true);
        }

        _ = MinionAnimCmd.Rearrange(duration: 0.5f);
        accessor.SetManualRearranged();
        PetOrderSnapshotManager.TakeSnapshot(Owner);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .WithAttackerFx(() => NMinionDiveBombVfx.Create(Owner.Creature, cardPlay.Target))
            .Execute(choiceContext);
    }

    /*
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != Owner || card is not Sortie)
        {
            return true;
        }

        if (TailorMinionCmd.GetMinionCount<TailorMinion>(card.Owner) < 3)
        {
            return false;
        }

        return true;
    }
    */

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}