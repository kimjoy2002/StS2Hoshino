using BaseLib;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2Hoshino.StS2HoshinoCode.UI;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch]
public static class ReloadButtonPatch
{
    [HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi._Ready))]
    [HarmonyPostfix]
    public static void CombatUiReadyPostfix(NCombatUi __instance)
    {
        Node hudParent = (Node?)NRun.Instance?.GlobalUi ?? __instance;
        ReloadButtonHud? hud = hudParent.GetNodeOrNull<ReloadButtonHud>("ReloadButtonHUD") 
                               ?? __instance.GetNodeOrNull<ReloadButtonHud>("ReloadButtonHUD");
        if (hud == null)
        {
            hud = new ReloadButtonHud();
            hudParent.AddChildSafely(hud);
        }

        hud.Bind(__instance);
    }

    [HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
    [HarmonyPostfix]
    public static void CombatUiActivatePostfix(NCombatUi __instance, CombatState state)
    {
        StS2HoshinoMain.Logger.Info("[CombatManager] CombatUiActivatePostfix");
        Node hudParent = (Node?)NRun.Instance?.GlobalUi ?? __instance;
        ReloadButtonHud? hud = hudParent.GetNodeOrNull<ReloadButtonHud>("ReloadButtonHUD") 
                               ?? __instance.GetNodeOrNull<ReloadButtonHud>("ReloadButtonHUD");
        if (hud == null)
        {
            hud = new ReloadButtonHud();
            hudParent.AddChildSafely(hud);
        }
        StS2HoshinoMain.Logger.Info("[CombatManager] Activate");

        hud.Activate(state);
       // StS2HoshinoMain.Controller.OnCombatUiActivated(__instance, state);
    }
    //
    // [HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Deactivate))]
    // [HarmonyPostfix]
    // public static void CombatUiDeactivatePostfix(NCombatUi __instance)
    // {
    //     MainFile.Controller.OnCombatUiDeactivated(__instance);
    // }

    [HarmonyPatch(typeof(NGame), nameof(NGame._Input))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    public static bool GameInputPrefix(NGame __instance, Godot.InputEvent inputEvent)
    {
        NCombatUi? combatUi = NCombatRoom.Instance?.Ui;
        return combatUi == null || !StS2HoshinoMain.Controller.TryHandleHotkey(combatUi, inputEvent);
    }
}