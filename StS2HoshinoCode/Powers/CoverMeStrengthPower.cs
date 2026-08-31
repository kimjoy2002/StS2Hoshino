using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class CoverMeStrengthPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<CoverMe>();

    public string? CustomPackedIconPath => "cover_me_power.png".PowerImagePath();
    public string? CustomBigIconPath => "cover_me_power.png".BigPowerImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
}
