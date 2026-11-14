using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheTailor.Character;

namespace TheTailor.Cards.Rare;

[Pool(typeof(TheTailorCardPool))]
public class MagnumOpus() : CustomCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override int MaxUpgradeLevel => 99999;
    public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/magnumOpusBeta.png";
    public override string? PortraitPath => "res://TheTailor/images/card_portraits/magnumOpusBeta.png";
    public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/magnumOpusBeta.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(16m, ValueProp.Move) /*, new DynamicVar("Delicate", 2)*/];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        /*
        if (shouldTriggerFatal && attackCommand.Results.SelectMany((List<DamageResult> r) => r).Any((DamageResult r) => r.WasTargetKilled))
        {
            if (DeckVersion != null)
            {
                SafeUpgrade(DeckVersion);
            }
            CardCmd.Upgrade(this);
        }
        */
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m + (CurrentUpgradeLevel - 1));
        // DynamicVars["Delicate"].UpgradeValueBy(1m);
    }

    /*
    /// <summary>
    ///     Modified copy of CardCmd.Upgrade as it doesn't normally allow upgrading while combat 'is ending'
    /// </summary>
    public static void SafeUpgrade(CardModel cardModel, CardPreviewStyle style = CardPreviewStyle.HorizontalLayout)
    {
        CardModel[] cards = [cardModel];

        foreach (CardModel card in cards)
        {
            if (!card.IsUpgradable)
            {
                continue;
            }

            CardPile pile = card.Pile;
            if (pile != null && pile.Type == PileType.Deck)
            {
                card.Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(card.Owner.NetId).UpgradedCards.Add(card.Id);
            }

            card.UpgradeInternal();
            card.FinalizeUpgradeInternal();
            if (!LocalContext.IsMine(card))
            {
                continue;
            }

            pile = card.Pile;
            if (pile != null && pile.Type == PileType.Deck)
            {
                Control control;
                switch (style)
                {
                    case CardPreviewStyle.EventLayout:
                        control = NRun.Instance?.GlobalUi.EventCardPreviewContainer;
                        break;
                    case CardPreviewStyle.HorizontalLayout:
                        control = NRun.Instance?.GlobalUi.CardPreviewContainer;
                        break;
                    case CardPreviewStyle.MessyLayout:
                        control = NRun.Instance?.GlobalUi.MessyCardPreviewContainer;
                        break;
                    case CardPreviewStyle.GridLayout:
                        control = NRun.Instance?.GlobalUi.GridCardPreviewContainer;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("style", $"Unexpected {"CardPreviewStyle"} {style}!");
                    case CardPreviewStyle.None:
                        continue;
                }

                AddChildSafely(control, NCardUpgradeVfx.Create(card));
            }
        }
    }

    /// <summary>
    ///     Child
    /// </summary>
    public static void AddChildSafely(Node parent, Node? child)
    {
        if (child != null)
        {
            if (NGame.IsMainThread() && (parent.IsNodeReady() || !parent.IsInsideTree()))
            {
                parent.AddChild(child, forceReadableName: false, Node.InternalMode.Disabled);
                return;
            }
            parent.CallDeferred(Node.MethodName.AddChild, child);
        }
    }
    */
}