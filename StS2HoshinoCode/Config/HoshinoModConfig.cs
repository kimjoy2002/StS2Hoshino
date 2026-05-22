using BaseLib.Config;

namespace StS2Hoshino.StS2HoshinoCode.Config;

[ConfigHoverTipsByDefault]
public class HoshinoModConfig : SimpleModConfig
{
    
    public enum ReloadButtonPosition
    {
        Up,
        Down,
        Left,
        Right
    };
    
    [ConfigSection("ReloadSettings")]
    
    [ConfigHoverTip]
    public static bool IsVewingReloadButton { get; set; } = true;
    
    [ConfigVisibleIf(nameof(IsVewingReloadButton))]
    [ConfigHoverTip]
    public static ReloadButtonPosition ButtonPosition { get; set; } = ReloadButtonPosition.Up;
    
}