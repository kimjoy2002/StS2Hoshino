using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Cards.Basic;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Compatibility;

internal static class UndoAndRestartCompatibility
{
    private const string UndoAssemblyName = "UndoAndRestart";
    private const string CombatSnapshotTypeName = "UndoAndRestartCode.CombatSnapshot";
    private const string UndoRedoManagerTypeName = "UndoAndRestartCode.UndoRedoManager";

    private static readonly ConditionalWeakTable<object, AmmoClass.CombatSnapshot> AmmoSnapshots = new();
    private static readonly object InstallLock = new();

    private static Harmony? _harmony;
    private static bool _installed;
    private static bool _assemblyLoadSubscribed;
    private static MethodInfo? _requestForcedCapture;
    private static bool _manualCaptureLookupComplete;
    private static MethodInfo? _captureBeforeAction;
    private static MethodInfo? _captureAfterActionWithEntry;
    private static ConstructorInfo? _historyEntryConstructor;
    private static Type? _historyEntryKindType;

    internal static void Initialize(Harmony harmony)
    {
        _harmony = harmony;
        if (TryInstall())
        {
            return;
        }

        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
        _assemblyLoadSubscribed = true;
    }

    internal static void RequestManualReloadCheckpoint()
    {
        try
        {
            if (!_manualCaptureLookupComplete)
            {
                Type? managerType = AccessTools.TypeByName(UndoRedoManagerTypeName);
                _requestForcedCapture = managerType?.GetMethod(
                    "RequestForcedPlayerControlReadyCapture",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new[] { typeof(string) },
                    null);
                _manualCaptureLookupComplete = managerType != null;
            }

            _requestForcedCapture?.Invoke(null, new object[] { "Hoshino manual reload" });
        }
        catch (Exception ex)
        {
            _requestForcedCapture = null;
            _manualCaptureLookupComplete = true;
            SafeWarn($"Manual reload checkpoint was skipped: {ex.GetBaseException().Message}");
        }
    }

    internal static bool BeginManualReload(GameAction action)
    {
        try
        {
            ResolveManualHistoryApi();
            if (_captureBeforeAction == null || _captureAfterActionWithEntry == null ||
                _historyEntryConstructor == null || _historyEntryKindType == null)
            {
                return false;
            }

            _captureBeforeAction.Invoke(null, new object[] { action, "Hoshino manual reload" });
            return true;
        }
        catch (Exception ex)
        {
            SafeWarn($"Manual reload history start was skipped: {ex.GetBaseException().Message}");
            return false;
        }
    }

    internal static async Task CompleteManualReloadAsync(GameAction action, Player player, bool historyStarted)
    {
        if (!historyStarted)
        {
            RequestManualReloadCheckpoint();
            return;
        }

        try
        {
            CardModel displayCard = ModelDb.Card<QuickReloadSkill>();
            string title = new LocString("gameplay_ui", "RELOAD_BUTTON").GetFormattedText() ?? displayCard.Title;
            string detail = player.Creature?.LogName ?? string.Empty;
            int turnNumber = CombatManager.Instance.DebugOnlyGetState()?.RoundNumber ?? 1;
            object cardKind = Enum.Parse(_historyEntryKindType!, "Card");
            object entry = _historyEntryConstructor!.Invoke(new object?[]
            {
                cardKind,
                title,
                detail,
                turnNumber,
                displayCard,
                null,
                null
            });

            object? result = _captureAfterActionWithEntry!.Invoke(null, new object[]
            {
                Task.CompletedTask,
                action,
                "Hoshino manual reload",
                entry
            });
            if (result is not Task captureTask)
            {
                throw new InvalidOperationException("Undo history completion did not return a Task.");
            }

            await captureTask;
        }
        catch (Exception ex)
        {
            SafeWarn($"Manual reload history was skipped: {ex.GetBaseException().Message}");
            RequestManualReloadCheckpoint();
        }
    }

    private static void ResolveManualHistoryApi()
    {
        if (_manualCaptureLookupComplete && _captureBeforeAction != null)
        {
            return;
        }

        Type? managerType = AccessTools.TypeByName(UndoRedoManagerTypeName);
        if (managerType == null)
        {
            return;
        }

        _requestForcedCapture = managerType.GetMethod(
            "RequestForcedPlayerControlReadyCapture",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(string) },
            null);
        _captureBeforeAction = managerType.GetMethod(
            "CaptureBeforeAction",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(GameAction), typeof(string) },
            null);

        Type? entryType = AccessTools.TypeByName("UndoAndRestartCode.ActionHistoryEntry");
        _historyEntryKindType = AccessTools.TypeByName("UndoAndRestartCode.ActionHistoryEntryKind");
        if (entryType != null && _historyEntryKindType != null)
        {
            _historyEntryConstructor = entryType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(constructor => constructor.GetParameters().Length == 7);
            _captureAfterActionWithEntry = managerType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(method =>
                {
                    if (method.Name != "CaptureAfterActionAsync")
                    {
                        return false;
                    }

                    ParameterInfo[] parameters = method.GetParameters();
                    return parameters.Length == 4 &&
                           parameters[0].ParameterType == typeof(Task) &&
                           parameters[1].ParameterType == typeof(GameAction) &&
                           parameters[2].ParameterType == typeof(string) &&
                           parameters[3].ParameterType == entryType;
                });
        }

        _manualCaptureLookupComplete = true;
    }

    private static void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
        if (!string.Equals(args.LoadedAssembly.GetName().Name, UndoAssemblyName, StringComparison.Ordinal))
        {
            return;
        }

        TryInstall();
    }

    private static bool TryInstall()
    {
        lock (InstallLock)
        {
            if (_installed)
            {
                return true;
            }

            Type? snapshotType = AccessTools.TypeByName(CombatSnapshotTypeName);
            if (snapshotType == null || _harmony == null)
            {
                return false;
            }

            try
            {
                MethodInfo? captureMethod = snapshotType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .FirstOrDefault(method =>
                        method.Name == "Capture" &&
                        method.ReturnType == snapshotType &&
                        method.GetParameters().Any(parameter => parameter.ParameterType == typeof(CombatState)));
                MethodInfo? restoreMethod = snapshotType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(method => method.Name == "RestoreAsync" && method.ReturnType == typeof(Task));

                if (captureMethod == null || restoreMethod == null)
                {
                    SafeWarn(
                        "Undo And Restart was found, but its snapshot API is not compatible. Hoshino will continue without undo integration.");
                    UnsubscribeAssemblyLoad();
                    return false;
                }

                _harmony.Patch(
                    captureMethod,
                    postfix: new HarmonyMethod(typeof(UndoAndRestartCompatibility), nameof(CapturePostfix)));
                _harmony.Patch(
                    restoreMethod,
                    postfix: new HarmonyMethod(typeof(UndoAndRestartCompatibility), nameof(RestorePostfix)));

                _installed = true;
                _manualCaptureLookupComplete = false;
                UnsubscribeAssemblyLoad();
                SafeInfo("Hoshino combat state integration enabled.");
                return true;
            }
            catch (Exception ex)
            {
                SafeWarn($"Integration was disabled: {ex.GetBaseException().Message}");
                UnsubscribeAssemblyLoad();
                return false;
            }
        }
    }

    private static void UnsubscribeAssemblyLoad()
    {
        if (!_assemblyLoadSubscribed)
        {
            return;
        }

        AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;
        _assemblyLoadSubscribed = false;
    }

    private static void CapturePostfix(object? __result, object[] __args)
    {
        try
        {
            CombatState? state = __args.OfType<CombatState>().FirstOrDefault();
            if (__result == null || state == null)
            {
                return;
            }

            AmmoSnapshots.Remove(__result);
            AmmoSnapshots.Add(__result, AmmoClass.CaptureCombatSnapshot(state.Players));
        }
        catch (Exception ex)
        {
            SafeWarn($"Hoshino state capture was skipped: {ex.GetBaseException().Message}");
        }
    }

    private static void RestorePostfix(object __instance, ref Task __result)
    {
        __result = RestoreAfterUndoAsync(__result, __instance);
    }

    private static async Task RestoreAfterUndoAsync(Task original, object snapshot)
    {
        await original;

        try
        {
            if (AmmoSnapshots.TryGetValue(snapshot, out AmmoClass.CombatSnapshot? ammoSnapshot))
            {
                AmmoClass.RestoreCombatSnapshot(ammoSnapshot);
            }
        }
        catch (Exception ex)
        {
            SafeWarn($"Hoshino state restore was skipped: {ex.GetBaseException().Message}");
        }
    }

    private static void SafeInfo(string message)
    {
        try
        {
            StS2HoshinoMain.Logger.Info($"[UndoCompatibility] {message}");
        }
        catch
        {
            // Compatibility diagnostics must never prevent the base mod from loading.
        }
    }

    private static void SafeWarn(string message)
    {
        try
        {
            StS2HoshinoMain.Logger.Warn($"[UndoCompatibility] {message}");
        }
        catch
        {
            // Compatibility diagnostics must never prevent the base mod from loading.
        }
    }
}
