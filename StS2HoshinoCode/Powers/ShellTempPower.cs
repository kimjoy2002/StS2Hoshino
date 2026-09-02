using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Relics;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class ShellTempPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Relic<ShellBeltRelic>();

    public string? CustomPackedIconPath => "shell_temp_power.png".PowerImagePath();

    public string? CustomBigIconPath => "shell_temp_power.png".BigPowerImagePath();
}
