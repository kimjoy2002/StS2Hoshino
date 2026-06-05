using BaseLib.Abstracts;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using Godot;

namespace StS2Hoshino.StS2HoshinoCode.Character;

public class StS2HoshinoPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => StS2Hoshino.Color;

    public override string BigEnergyIconPath => "charui/hoshino_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/hoshino_text_energy.png".ImagePath();
}