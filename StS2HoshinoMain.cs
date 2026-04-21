using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using StS2Hoshino.StS2HoshinoCode.Core;

namespace StS2Hoshino;

[ModInitializer(nameof(Initialize))]
public partial class StS2HoshinoMain : Node
{
    public static ReloadController Controller { get; } = new();

    public const string ModId = "StS2Hoshino";
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
         new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
        
    }
}