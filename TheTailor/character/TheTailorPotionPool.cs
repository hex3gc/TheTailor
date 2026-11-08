using BaseLib.Abstracts;
using Godot;

namespace TheTailor.Character;

public class TheTailorPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => TheTailor.Color;

    public override string BigEnergyIconPath => "res://TheTailor/images/charui/big_energy.png";
    public override string TextEnergyIconPath => "res://TheTailor/images/charui/text_energy.png";
}