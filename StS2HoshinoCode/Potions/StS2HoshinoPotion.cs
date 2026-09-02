using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Potions;

public abstract class StS2HoshinoPotion : CustomPotionModel
{
    private string PotionImagePath => $"potions/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ImagePath();
    private string PotionOutlinePath => $"potions/{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".ImagePath();

    public override string? CustomPackedImagePath =>
        ResourceLoader.Exists(PotionImagePath) ? PotionImagePath : null;

    public override string? CustomPackedOutlinePath =>
        ResourceLoader.Exists(PotionOutlinePath) ? PotionOutlinePath : null;

}
