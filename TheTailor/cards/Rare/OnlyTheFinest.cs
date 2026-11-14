using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class OnlyTheFinest() : CustomCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/onlyTheFinestBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/onlyTheFinestBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/onlyTheFinestBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var item in GetCards().ToList())
        {
            await CardCmd.Exhaust(choiceContext, item);
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1);
    }

    private IEnumerable<CardModel> GetCards()
    {
        var pile = PileType.Hand.GetPile(Owner);
        return pile.Cards.Where
        (c =>
            {
                if (c is Wither)
                {
                    var w = c as Wither;
                    if (w.FakeUpgradeLevel <= 0) return true;
                }
                else if (!c.IsUpgraded)
                {
                    return true;
                }

                return false;
            }
        );
    }
}