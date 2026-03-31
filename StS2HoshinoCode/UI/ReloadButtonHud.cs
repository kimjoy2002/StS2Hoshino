using System;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using StS2Hoshino.StS2HoshinoCode.Core;

namespace StS2Hoshino.StS2HoshinoCode.UI;

public partial class ReloadButtonHud : Control
{
    private readonly ClickableButton? _reloadButton;
    private NCombatUi? _combatUi;
    private CombatState? _state;

    
    private Button _debugButton = null!;
    public ReloadButtonHud()
    {
        Name = "ReloadButtonHUD";
        MouseFilter = MouseFilterEnum.Ignore;
        ProcessMode = Node.ProcessModeEnum.Inherit;
        TopLevel = true;
        ZAsRelative = false;
        ZIndex = 100;
        SetAnchorsPreset(LayoutPreset.TopLeft);

        _reloadButton = CreateButton("ReloadButton");

        if (_reloadButton == null)
        {
            StS2HoshinoMain.Logger.Error("ReloadButton creation failed: _reloadButton is null");
            return;
        }

        _reloadButton.Position = Vector2.Zero;
        _reloadButton.Activated += OnReloadPressed;
        AddChild(_reloadButton);
    }

    public void Bind(NCombatUi combatUi)
    {
        _combatUi = combatUi;
    }
    public void Activate(CombatState state)
    {
        _state = state;
        StS2HoshinoMain.Controller.OnCombatActivated(state);
    }


    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(220f, 90f);
        Size = CustomMinimumSize;
    }

    public override void _Process(double delta)
    {
        Player? me = null;
        try
        {
            me = LocalContext.GetMe(_state);
        }
        catch (InvalidOperationException ignored)
        {
        }

        
        if (me == null ||  !IsInstanceValid(_combatUi) || _combatUi == null || !IsInstanceValid(_combatUi)
            || !(me.Character is Character.StS2Hoshino))
        {
            Visible = false;
            return;
        }

        ReloadController controller = StS2HoshinoMain.Controller;
        Visible = controller.ShouldShowHud(_combatUi);
        if (!Visible)
            return;

        MoveToFront();
        Vector2 energyPosition = _combatUi.EndTurnButton.GetGlobalRect().Position;
        Position = energyPosition + new Vector2(-4f, -Size.Y);
        if (_reloadButton != null)
        {
            _reloadButton.IsEnabled = controller.CanReload(_combatUi);
            _reloadButton.HoverTitleSource = new LocString("static_hover_tips", "RELOAD_BUTTON.title");
            _reloadButton.HoverDescriptionSource = new LocString("static_hover_tips", "RELOAD_BUTTON.description");
        }
    }

    private void OnReloadPressed()
    {
        if (_combatUi == null) return;

        StS2HoshinoMain.Logger.Info("OnReloadPressed!!");
        StS2HoshinoMain.Controller.Reload(_combatUi);
    }

    private static ClickableButton CreateButton(string name)
    {
        StS2HoshinoMain.Logger.Info($"CreateButton called: {name}");
        var button = new ClickableButton
        {
            Name = name
        };
        return button;
    }
}
