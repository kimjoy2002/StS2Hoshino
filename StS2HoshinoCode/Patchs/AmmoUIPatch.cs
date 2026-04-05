using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2Hoshino.StS2HoshinoCode.UI;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch(typeof(NCreature))]
public static class AmmoUIPatch
{
    
    [HarmonyPatch(typeof(CombatManager), nameof(CombatManager.StartCombatInternal))]
    [HarmonyPrefix]
    public static void CombatUiReadyPostfix(CombatManager __instance, CombatState? ____state)
    {
        StS2HoshinoMain.Logger.Info("[CombatManager] CombatUiReadyPostfix");
        if (____state == null)
        {
            StS2HoshinoMain.Logger.Info("[CombatManager] _state is null");
            return;
        }
        StS2HoshinoMain.Logger.Info($"[CombatManager] playersStartingTurn {____state.Players.Count}");
        foreach (var player in ____state.Players)
        {
            StS2HoshinoMain.Logger.Info($"[CombatManager] ResetFull {player.ToString()}");
            AmmoClass.ResetFull(player);
        }
    }
    
    //[HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    private static void AddAmmoUI(NCreature __instance)
    {
        StS2HoshinoMain.Logger.Info("[CombatManager] AddAmmoUI");
        if (__instance.Entity.IsPlayer && __instance.Entity.Player != null && LocalContext.IsMe(__instance.Entity) && __instance.Entity.Player.Character is Character.StS2Hoshino)
        {
            StS2HoshinoMain.Logger.Info("[CombatManager] AddAmmoUI 2");
            AmmoUINode ammoUINode = AmmoUINode.Create(__instance.Entity.Player);
            ((Node)ammoUINode).Name = "ammoUI";
            ((Node)ammoUINode).UniqueNameInOwner = true;
            GodotTreeExtensions.AddChildSafely((Node)(object)__instance, (Node)(object)ammoUINode);
            ((Node)__instance).MoveChild((Node)(object)ammoUINode, 0);
        }
    }
}
