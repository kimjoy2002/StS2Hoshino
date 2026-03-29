using BaseLib.Config;

namespace StS2Hoshino.StS2HoshinoCode.Config;

public class ModConfig : SimpleModConfig {
    [ConfigHoverTip]
    public static bool TestConfig { get; set; } = true;
}