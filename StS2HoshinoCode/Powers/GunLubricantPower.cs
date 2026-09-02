using MegaCrit.Sts2.Core.Entities.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class GunLubricantPower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
