using BaseLib.Abstracts;
using BaseLib.Extensions;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using Godot;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public abstract class StS2HoshinoPower : CustomPowerModel
{
    //Loads from StS2Hoshino/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}