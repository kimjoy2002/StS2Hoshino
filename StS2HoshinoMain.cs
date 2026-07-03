using System.Reflection;
using BaseLib.Audio;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using StS2Hoshino.StS2HoshinoCode.Core;
using BaseLib.Config;
using MegaCrit.Sts2.Core.Combat;
using StS2Hoshino.StS2HoshinoCode.Config;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino;

[ModInitializer(nameof(Initialize))]
public partial class StS2HoshinoMain : Node
{
    public static ReloadController Controller { get; } = new();

    public const string ModId = "StS2Hoshino";
    public static readonly AutoModAudio Audio = new($"res://{ModId}/audio");
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
         new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static float ShotgunVolumeMultiplier = 0.10f;
    public static float ReloadVolumeMultiplier = 1.0f;

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        ModConfigRegistry.Register(ModId, new HoshinoModConfig());
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
        
        CombatManager.Instance.TurnStarted += OnTurnStarted;
    }

    private static void OnTurnStarted(CombatState state)
    {
        if (state.CurrentSide == CombatSide.Player)
        {
            foreach (var player in state.Players)
            {
                AmmoClass.ResetForTurnStart(player);
            }
        }
    }

    public static void PlaySfx(string path, float volumeMult = 1f)
    {
        float multiplier = path.Contains("shotgunfire")
            ? StS2HoshinoMain.ShotgunVolumeMultiplier
            : StS2HoshinoMain.ReloadVolumeMultiplier;

        Audio.PlaySfx(path, volumeMult: volumeMult * multiplier);
    }
}
