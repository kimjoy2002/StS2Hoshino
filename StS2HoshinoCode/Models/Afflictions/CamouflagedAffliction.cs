using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Helpers;
using HarmonyLib;

namespace StS2Hoshino.StS2HoshinoCode.Models.Afflictions;

public class CamouflagedAffliction : AfflictionModel
{
}

[HarmonyPatch(typeof(AfflictionModel), "OverlayPath", MethodType.Getter)]
public static class CamouflagedAfflictionOverlayPatch
{
    public static void Postfix(AfflictionModel __instance, ref string __result)
    {
        if (__instance is CamouflagedAffliction)
        {
            __result = "res://StS2Hoshino/scenes/camouflaged.tscn";
        }
    }
}
