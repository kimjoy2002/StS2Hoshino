using System;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Actions;

namespace StS2Hoshino.StS2HoshinoCode.Core;


public sealed partial class ReloadController
{
    private CombatState? _activeCombatState;

    public void OnCombatActivated(CombatState state)
    {
        _activeCombatState = state;
    }

    public bool ShouldShowHud(NCombatUi combatUi)
    {
        return GodotObject.IsInstanceValid(combatUi)
               && combatUi.IsVisibleInTree()
               && IsSinglePlayerCombat()
               && CombatManager.Instance.IsInProgress
               && !IsHudHiddenByNonChoiceUi(combatUi);
    }

    private static bool IsSinglePlayerCombat()
    {
        return RunManager.Instance.IsSinglePlayerOrFakeMultiplayer
               && CombatManager.Instance.IsInProgress
               && NGame.Instance?.CurrentRunNode != null;
    }

    public bool TryHandleHotkey(NCombatUi combatUi, InputEvent inputEvent)
    {
        if (!ShouldShowHud(combatUi))
            return false;

        if (inputEvent is not InputEventKey keyEvent || !keyEvent.Pressed || keyEvent.Echo)
            return false;

        if (keyEvent.CtrlPressed || keyEvent.AltPressed || keyEvent.MetaPressed)
            return false;

        if (MatchesHotkey(keyEvent, Key.R))
        {
            Reload(combatUi);
            return true;
        }

        return false;
    }

    private static bool MatchesHotkey(InputEventKey keyEvent, Key key)
    {
        return keyEvent.Keycode == key || keyEvent.PhysicalKeycode == key || keyEvent.KeyLabel == key;
    }

    private static bool IsSupportedChoiceUiActive(NCombatUi combatUi)
    {
        if (NCombatRoom.Instance?.Ui != combatUi)
            return false;

        if (combatUi.Hand.IsInCardSelection)
            return true;

        return NOverlayStack.Instance?.Peek() is NChooseACardSelectionScreen or NCardGridSelectionScreen;
    }

    private static bool IsHudHiddenByNonChoiceUi(NCombatUi combatUi)
    {
        if (IsSupportedChoiceUiActive(combatUi))
            return false;

        if (combatUi.Hand.IsInCardSelection)
            return false;

        if (NOverlayStack.Instance?.ScreenCount > 0)
            return true;

        object? currentScreen = ActiveScreenContext.Instance.GetCurrentScreen();
        if (currentScreen == null)
            return false;

        if (ReferenceEquals(currentScreen, NCombatRoom.Instance) || ReferenceEquals(currentScreen, combatUi))
            return false;

        if (currentScreen is Node screenNode)
        {
            if (ReferenceEquals(screenNode, NCombatRoom.Instance)
                || ReferenceEquals(screenNode, combatUi)
                || screenNode.IsAncestorOf(combatUi))
            {
                return false;
            }
        }

        return true;
    }

    public bool CanReload(NCombatUi combatUi)
    {
        return ShouldShowHud(combatUi)
               && CanRestoreState()
               && !IsUiBlocking(combatUi);
    }

    public void Reload(NCombatUi combatUi)
    {
        if (!CanReload(combatUi))
            return;

        Player? me = null;
        try
        {
            me = LocalContext.GetMe(_activeCombatState);
        }
        catch (InvalidOperationException ex)
        {
            StS2HoshinoMain.Logger.Error($"[ReloadController] Could not find local player: {ex.Message}");
            return;
        }

        if (me == null)
        {
            StS2HoshinoMain.Logger.Error("[ReloadController] Local player is null, cannot reload");
            return;
        }

        StS2HoshinoMain.Logger.Info($"[ReloadController] Enqueueing ReloadAction for player {me.NetId}");
        ReloadAction action = new ReloadAction(me);
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(action);
    }

    private bool IsUiBlocking(NCombatUi combatUi)
    {
        if (!CombatManager.Instance.EndingPlayerTurnPhaseOne && !CombatManager.Instance.EndingPlayerTurnPhaseTwo)
            return false;

        GameAction? action = RunManager.Instance.ActionExecutor.CurrentlyRunningAction;
        return action?.State != GameActionState.GatheringPlayerChoice || !IsSupportedChoiceUiActive(combatUi);
    }

    private bool CanRestoreState()
    {
        return true;
    }
}