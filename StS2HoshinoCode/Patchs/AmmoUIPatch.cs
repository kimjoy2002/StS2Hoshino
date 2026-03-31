using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2Hoshino.StS2HoshinoCode.UI;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch(typeof(NCreature))]
public static class AmmoUIPatch
{
    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    private static void AddAmmoUI(NCreature __instance)
    {
        if (__instance.Entity.IsPlayer && __instance.Entity.Player != null && LocalContext.IsMe(__instance.Entity) && __instance.Entity.Player.Character is Character.StS2Hoshino)
        {
            AmmoUINode ammoUINode = AmmoUINode.Create(__instance.Entity.Player);
            ((Node)ammoUINode).Name = "ammoUI";
            ((Node)ammoUINode).UniqueNameInOwner = true;
            GodotTreeExtensions.AddChildSafely((Node)(object)__instance, (Node)(object)ammoUINode);
            ((Node)__instance).MoveChild((Node)(object)ammoUINode, 0);
        }
    }
}
