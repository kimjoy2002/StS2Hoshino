using BaseLib.Abstracts;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using Godot;

namespace StS2Hoshino.StS2HoshinoCode.Character;

public class StS2HoshinoRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => StS2Hoshino.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}