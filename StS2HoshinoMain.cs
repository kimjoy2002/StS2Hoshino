using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace StS2Hoshino;

[ModInitializer(nameof(Initialize))]
public partial class StS2HoshinoMain : Node
{

    public const string ModId = "StS2Hoshino";
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
         new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
        
    }
}