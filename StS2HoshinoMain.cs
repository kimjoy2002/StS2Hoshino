using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Commands;
using StS2Hoshino.StS2HoshinoCode.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace StS2Hoshino;

[ModInitializer(nameof(Initialize))]
public partial class StS2HoshinoMain : Node
{
    public static ReloadController Controller { get; } = new();

    public const string ModId = "StS2Hoshino";
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
         new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static float ShotgunVolumeMultiplier = 0.15f;
    public static float ReloadVolumeMultiplier = 1.0f;

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
    }

    public static void PlayCustomSfx(string path, float volume)
    {
        Callable.From(() =>
        {
            var mainLoop = Engine.GetMainLoop() as SceneTree;
            var rootNode = mainLoop?.Root;
            if (rootNode == null) return;

            var player = new AudioStreamPlayer();
            rootNode.AddChild(player);

            var stream = GD.Load<AudioStream>(path);
            if (stream != null)
            {
                player.Stream = stream;
                
                float multiplier = path.Contains("shotgunfire") ? ShotgunVolumeMultiplier : ReloadVolumeMultiplier;
                player.VolumeDb = (float)Mathf.LinearToDb(volume * multiplier);

                player.Finished += () => player.QueueFree();
                player.Play();
            }
            else
            {
                player.QueueFree();
                GD.PrintErr($"[StS2Hoshino] Failed to load audio stream: {path}");
            }
        }).CallDeferred();
    }
}

[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.Play), typeof(string), typeof(float))]
public static class SfxCmdPatch
{
    public static bool Prefix(string sfx, float volume)
    {
        if (sfx != null && sfx.StartsWith("res://"))
        {
            StS2HoshinoMain.PlayCustomSfx(sfx, volume);
            return false; // Skip original
        }
        return true; // Continue to original
    }
}