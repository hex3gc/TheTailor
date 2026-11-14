using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheTailor.Character;
using TheTailor.Minions;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class Repurpose() : CustomCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/repurposeBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/repurposeBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/repurposeBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);

        var replaceIndex = -1;
        var accessor = new PetsOrderAccessor(Owner);
        if (accessor.Pets != null)
        {
            foreach (var creature in accessor.Pets)
                if (creature.Monster is TailorMinion)
                {
                    replaceIndex = accessor.Pets.IndexOf(creature);
                    break;
                }

            if (replaceIndex == -1) return;

            accessor.Pets[replaceIndex].RemoveAllPowersInternalExcept();
            await CreatureCmd.Kill(accessor.Pets[replaceIndex], true);
            _ = MinionAnimCmd.Rearrange(duration: 0.5f);
            accessor.SetManualRearranged();
            PetOrderSnapshotManager.TakeSnapshot(Owner);

            await TailorMinionCmd.PutOstyAtBack(choiceContext, Owner);
        }
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != Owner || card is not Repurpose) return true;

        if (TailorMinionCmd.GetMinionCount<TailorMinion>(card.Owner) <= 0) return false;

        return true;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}