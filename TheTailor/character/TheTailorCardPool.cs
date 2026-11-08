using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace TheTailor.Character;

public class TheTailorCardPool : CustomCardPoolModel
{
    public override string Title => TheTailor.CharacterId;
    public override string BigEnergyIconPath => "res://TheTailor/images/charui/big_energy.png";
    public override string TextEnergyIconPath => "res://TheTailor/images/charui/text_energy.png";
    public override float H => 1f;
    public override float S => 1f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => TheTailor.Color;
    public override bool IsColorless => false;

    public override Texture2D CustomFrame(CustomCardModel card)
    {
        var attackFrame = PreloadManager.Cache.GetTexture2D("res://TheTailor/images/cards/attackframe.png");
        var defaultFrame = PreloadManager.Cache.GetTexture2D("res://TheTailor/images/cards/skillframe.png");
        var powerFrame = PreloadManager.Cache.GetTexture2D("res://TheTailor/images/cards/powerframe.png");

        return card.Type switch
        {
            CardType.Attack => attackFrame,
            CardType.Skill => defaultFrame,
            CardType.Power => powerFrame,
            CardType.Curse => defaultFrame,
            CardType.Status => defaultFrame,
            CardType.Quest => defaultFrame,
            CardType.None => attackFrame,
            _ => defaultFrame
        };
    }
}