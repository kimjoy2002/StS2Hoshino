using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace StS2Hoshino.StS2HoshinoCode.UI;

public partial class ClickableButton : Control
{
    private HoverTip _hoverTip;
    private static readonly Vector2 _hoverTipOffset = new Vector2(-76f, -122f);
    private bool _hovered;
    private bool _pressed;
    private bool _enabled = true;
    private float _glowTime;

    private Texture2D _glowTexture;
    private Texture2D _normalTexture;
    private Font _font;

    private static string ButtonPath => "res://StS2Hoshino/images/charui/reload_turn_button.png";
    private static string ButtonGlowPath => "res://StS2Hoshino/images/charui/reload_turn_button_glow.png";
    private static string FontPath => "StS2Hoshino/etc//kreon_bold_shared.tres";
    
    private readonly LocString _labelText = new LocString("gameplay_ui", "RELOAD_BUTTON");
    private LocString _hoverTitle = new LocString("static_hover_tips", "RELOAD_BUTTON.title");
    private LocString _hoverDescription = new LocString("static_hover_tips", "RELOAD_BUTTON.description");

    public ClickableButton()
    {
        MouseFilter = MouseFilterEnum.Stop;
        FocusMode = FocusModeEnum.None;
        MouseDefaultCursorShape = CursorShape.PointingHand;

        _hoverTip = new HoverTip(new LocString("static_hover_tips", "RELOAD_BUTTON.title"), new LocString("static_hover_tips", "RELOAD_BUTTON.description"));
        CustomMinimumSize = new Vector2(220f, 90f);
        Size = CustomMinimumSize;

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        _glowTexture = PreloadManager.Cache.GetCompressedTexture2D(ButtonGlowPath);
        _normalTexture = PreloadManager.Cache.GetCompressedTexture2D(ButtonPath);
        _font = GD.Load<Font>(FontPath);

        SetProcess(true);
    }

    public event Action? Activated;

    public bool IsEnabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value)
                return;

            _enabled = value;
            MouseDefaultCursorShape = value ? CursorShape.PointingHand : CursorShape.Arrow;
            QueueRedraw();
        }
    }

    public string HoverTitle => _hoverTitle.GetFormattedText();

    public LocString HoverTitleSource
    {
        get => _hoverTitle;
        set
        {
            _hoverTitle = value;
            RefreshHoverTip();
        }
    }

    public string HoverDescription => _hoverDescription.GetFormattedText();

    public LocString HoverDescriptionSource
    {
        get => _hoverDescription;
        set
        {
            _hoverDescription = value;
            RefreshHoverTip();
        }
    }
    public string LabelText => _labelText.GetFormattedText();

    public override void _Process(double delta)
    {
        if (_hovered && _enabled)
        {
            _glowTime += (float)delta * 4.0f;
            QueueRedraw();
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (!_enabled)
            return;

        if (@event is not InputEventMouseButton mouseEvent || mouseEvent.ButtonIndex != MouseButton.Left)
            return;

        if (mouseEvent.Pressed)
        {
            _pressed = true;
            QueueRedraw();
            AcceptEvent();
            return;
        }

        bool shouldActivate = _pressed && _hovered;
        _pressed = false;
        QueueRedraw();
        AcceptEvent();

        if (shouldActivate)
            Activated?.Invoke();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationMouseExit)
        {
            _pressed = false;
            QueueRedraw();
        }
        else if (what == NotificationExitTree)
        {
            NHoverTipSet.Remove(this);
        }
    }

    public override void _Draw()
    {
        Rect2 rect = new(Vector2.Zero, Size);
        DrawButtonTexture(rect);
    }

    private void DrawButtonTexture(Rect2 rect)
    {

        if (_normalTexture != null)
        {
            Rect2 imageRect = rect;

            if (_pressed)
                imageRect.Position += new Vector2(0f, 3f);
            else if (_hovered)
                imageRect.Position += new Vector2(0f, -1f);

            Color modulate = !_enabled
                ? new Color(0.55f, 0.55f, 0.55f, 1f)
                : _pressed
                    ? new Color(0.82f, 0.82f, 0.82f, 1f)
                    : Colors.White;

            DrawTextureRect(_normalTexture, imageRect, false, modulate);
        }
        DrawButtonLabel(rect);
    }

    private void DrawButtonLabel(Rect2 rect)
    {
        if (_font == null)
            return;

        const int fontSize = 30;

        Vector2 textSize = _font.GetStringSize(LabelText, HorizontalAlignment.Center, -1, fontSize);

        Vector2 textPos = new Vector2(
            rect.GetCenter().X - textSize.X * 0.5f,
            rect.GetCenter().Y + 12f
        );

        if (_pressed)
            textPos += new Vector2(0f, 3f);
        else if (_hovered)
            textPos += new Vector2(0f, -1f);

        Color textColor = !_enabled
            ? new Color(0.57f, 0.56f, 0.50f, 1f)
            : _hovered
                ? new Color(1.0f, 0.22f, 0.18f, 1f)   // hover 시 빨간색
                : new Color(1f, 0.95f, 0.87f, 1f);

        Color outlineColor = !_enabled
            ? new Color(0.05f, 0.05f, 0.05f, 0.45f)
            : _hovered
                ? new Color(0.30f, 0.02f, 0.02f, 1f)
                : new Color(0.07f, 0.12f, 0.18f, 1f);

        Color shadowColor = new Color(0f, 0f, 0f, _hovered ? 0.30f : 0.22f);

        DrawString(_font, textPos + new Vector2(3f, 2f), LabelText, HorizontalAlignment.Left, -1, fontSize, shadowColor);
        DrawStringOutline(_font, textPos, LabelText, HorizontalAlignment.Left, -1, fontSize, 12, outlineColor);
        DrawString(_font, textPos, LabelText, HorizontalAlignment.Left, -1, fontSize, textColor);
    }


    private void OnMouseEntered()
    {
        _hovered = true;
        QueueRedraw();
        ShowHoverTip();
    }

    private void OnMouseExited()
    {
        _hovered = false;
        _pressed = false;
        QueueRedraw();
        NHoverTipSet.Remove(this);
    }

    private void RefreshHoverTip()
    {
        if (_hovered)
            ShowHoverTip();
    }

    private void ShowHoverTip()
    {
        NHoverTipSet.Remove(this);
        if (string.IsNullOrWhiteSpace(HoverTitle) && string.IsNullOrWhiteSpace(HoverDescription))
            return;

        NHoverTipSet.CreateAndShow(this, _hoverTip).GlobalPosition = GlobalPosition + _hoverTipOffset;
    }
}