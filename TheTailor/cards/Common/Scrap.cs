using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Cards.Token;
using TheTailor.Character;

namespace TheTailor.Cards.Common;

[Pool(typeof(TheTailorCardPool))]
public class Scrap() : CustomCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/scrap.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/scrap.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/scrapBeta.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Patch>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            CardModel card = CombatState.CreateCard<Patch>(Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, Owner));
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}