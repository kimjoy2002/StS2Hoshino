using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using StS2Hoshino.StS2HoshinoCode.UI;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch]
public static class AmmoCombatStartPatch
{
    private const string MethodName = "StartCombatInternal";
    private const string CombatTurnStateTypeName = "MegaCrit.Sts2.Core.Combat.CombatTurnState";
    private static readonly Type? CombatTurnStateType = AccessTools.TypeByName(CombatTurnStateTypeName);
    private static readonly PropertyInfo? CombatTurnStateStateProperty =
        CombatTurnStateType == null ? null : AccessTools.Property(CombatTurnStateType, "State");

    public static MethodBase? TargetMethod()
    {
        var methods = AccessTools.GetDeclaredMethods(typeof(CombatManager))
            .Where(method => method.Name == MethodName)
            .ToList();

        return methods.FirstOrDefault(method =>
                   method.GetParameters() is [{ ParameterType.FullName: CombatTurnStateTypeName }])
               ?? methods.FirstOrDefault(method => method.GetParameters().Length == 0);
    }

    public static bool Prepare() => TargetMethod() != null;

    [HarmonyPrefix]
    public static void ResetAmmoAtCombatStart(CombatManager __instance, object[] __args)
    {
        CombatState? state = __args.Length == 1
            ? CombatTurnStateStateProperty?.GetValue(__args[0]) as CombatState
            : null;
        state ??= __instance.DebugOnlyGetState();
        if (state == null)
        {
            return;
        }

        foreach (var player in state.Players)
        {
            AmmoClass.ResetFull(player);
        }
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
public static class AmmoUiPatch
{
    [HarmonyPostfix]
    private static void AddAmmoUI(NCreature __instance)
    {
        if (__instance.Entity.IsPlayer && __instance.Entity.Player != null && LocalContext.IsMe(__instance.Entity))
        {
            AmmoUINode ammoUINode = AmmoUINode.Create(__instance.Entity.Player);
            ((Node)ammoUINode).Name = "ammoUI";
            ((Node)ammoUINode).UniqueNameInOwner = true;
            GodotTreeExtensions.AddChildSafely((Node)(object)__instance, (Node)(object)ammoUINode);
            ((Node)__instance).MoveChild((Node)(object)ammoUINode, 0);

        }
    }
}
