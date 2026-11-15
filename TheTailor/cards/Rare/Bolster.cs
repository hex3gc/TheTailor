using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheTailor.Character;
using TheTailor.Minions;
using TheTailor.Powers;
using TheTailor.Relics.Common;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class Bolster() : CustomCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Minion };
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/bolsterBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/bolsterBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/bolsterBeta.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Power", 1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.BurlapMinion)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var result = await TailorMinionCmd.AddOrReplaceMinion<MinionBurlap>(choiceContext, Owner, true);
        if (result != null)
        {
            await PowerCmd.Apply<BolsterPower>(choiceContext, result, DynamicVars["Power"].IntValue, cardPlay.Player.Creature, null);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}