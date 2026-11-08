using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using TheTailor.Cards.Starter;
using TheTailor.Relics.Starter;

namespace TheTailor.Character;

public class TheTailor : PlaceholderCharacterModel
{
    public const string CharacterId = "TheTailor";

    public static readonly Color Color = new("5c350f");
    public override Color NameColor => Color;
    public override Color MapDrawingColor => Color;
    public override Color DialogueColor => Color;
    public override VfxColor SpeechBubbleColor => VfxColor.Purple;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 65;

    public override string CustomVisualPath => "res://TheTailor/scenes/char/tailor.tscn";
    public override string CustomRestSiteAnimPath => "res://TheTailor/scenes/rest_site/tailor_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://TheTailor/scenes/merchant/tailor_merchant.tscn";

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeTailor>(),
        ModelDb.Card<StrikeTailor>(),
        ModelDb.Card<StrikeTailor>(),
        ModelDb.Card<StrikeTailor>(),
        ModelDb.Card<DefendTailor>(),
        ModelDb.Card<DefendTailor>(),
        ModelDb.Card<DefendTailor>(),
        ModelDb.Card<DefendTailor>(),
        ModelDb.Card<Sew>(),
        ModelDb.Card<Craft>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<CraftKit>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<TheTailorCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<TheTailorRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<TheTailorPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "res://TheTailor/images/charui/character_icon_tailor.png";

    public override string CustomIconOutlineTexturePath =>
        "res://TheTailor/images/charui/character_icon_tailor_outline.png";

    public override string CustomCharacterSelectIconPath => "res://TheTailor/images/charui/char_select_tailor.png";

    public override string CustomCharacterSelectLockedIconPath =>
        "res://TheTailor/images/charui/char_select_tailor_locked.png";

    public override string CustomMapMarkerPath => "res://TheTailor/images/charui/map_marker_tailor.png";

    public override string CustomCharacterSelectTransitionPath =>
        "res://TheTailor/materials/transitions/tailor_transition_mat.tres";

    public override string CustomArmPointingTexturePath =>
        "res://TheTailor/images/charui/multiplayer_hand_tailor_point.png";

    public override string CustomArmRockTexturePath => "res://TheTailor/images/charui/multiplayer_hand_tailor_rock.png";

    public override string CustomArmPaperTexturePath =>
        "res://TheTailor/images/charui/multiplayer_hand_tailor_paper.png";

    public override string CustomArmScissorsTexturePath =>
        "res://TheTailor/images/charui/multiplayer_hand_tailor_scissors.png";

    public override string CustomCharacterSelectBg =>
        "res://TheTailor/scenes/screens/char_select/char_select_bg_tailor.tscn";

    public override string CustomEnergyCounterPath =>
        "res://TheTailor/scenes/combat/energy_counters/tailor_energy_counter.tscn";
}