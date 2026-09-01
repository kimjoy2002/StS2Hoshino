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
using StS2Hoshino.StS2HoshinoCode.Compatibility;
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

    public const float ShotgunVolumeMultiplier = 0.20f;
    public const float ReloadVolumeMultiplier = 1.0f;
    private const float DefaultVolumePercent = 100.0f;

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        ModConfigRegistry.Register(ModId, new HoshinoModConfig());
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
        UndoAndRestartCompatibility.Initialize(harmony);
        
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
        bool isShotgun = path.Contains("shotgunfire");
        float baseMultiplier = isShotgun
            ? ShotgunVolumeMultiplier
            : ReloadVolumeMultiplier;
        float configuredMultiplier = isShotgun
            ? HoshinoModConfig.BulletAttackVolumePercent / DefaultVolumePercent
            : HoshinoModConfig.ReloadVolumePercent / DefaultVolumePercent;

        Audio.PlaySfx(path, volumeMult: volumeMult * baseMultiplier * configuredMultiplier);
    }
}
