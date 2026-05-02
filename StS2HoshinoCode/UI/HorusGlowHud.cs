using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System.Linq;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.UI;

public partial class HorusGlowHud : Control
{
    private NCombatUi? _combatUi;
    private CombatState? _combatState;
    private Texture2D _glowTexture;
    
    private float _currentAlpha = 0f;
    private float _targetAlpha = 0f;
    private float _glowTime = 0f;

    public HorusGlowHud()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        _glowTexture = GD.Load<Texture2D>("res://StS2Hoshino/images/charui/horus_glow.png");
        
        Material = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.Add
        };
        
        SetProcess(true);
    }

    public void Bind(NCombatUi ui)
    {
        _combatUi = ui;
    }

    public void Activate(CombatState state)
    {
        _combatState = state;
    }

    public override void _Process(double delta)
    {
        if (_combatUi == null || _combatState == null || !CombatManager.Instance.IsInProgress)
        {
            Visible = false;
            return;
        }

        Player? me = null;
        try { me = LocalContext.GetMe(_combatState); } catch { }

        bool hasPower = me?.Creature?.Powers.OfType<HorusOfEyePower>().Any() == true;
        
        _targetAlpha = hasPower ? 1.0f : 0.0f;
        
        if (_currentAlpha != _targetAlpha)
        {
            _currentAlpha = Mathf.MoveToward(_currentAlpha, _targetAlpha, (float)delta * 1.0f);
        }

        if (_currentAlpha > 0f)
        {
            Visible = true;
            _glowTime += (float)delta * 1.0f;
            
            var drawPile = _combatUi.DrawPile;
            if (drawPile != null)
            {
                Rect2 drawPileRect = drawPile.GetGlobalRect();
                GlobalPosition = drawPileRect.GetCenter() + new Vector2(0f, -50f);
            }
            
            QueueRedraw();
        }
        else
        {
            Visible = false;
        }
    }

    public override void _Draw()
    {
        if (_glowTexture == null || _currentAlpha <= 0f) return;

        Vector2 texSize = _glowTexture.GetSize();
        float baseScale = 1.3f;
        Vector2 baseSize = texSize * baseScale;
        Rect2 imageRect = new Rect2(-baseSize / 2f, baseSize);

        Color baseGlowColor = new Color(0.2f, 0.9f, 1.0f, _currentAlpha);
        
        DrawTextureRect(_glowTexture, imageRect, false, baseGlowColor);
        DrawTextureRect(_glowTexture, imageRect, false, baseGlowColor);

        float progress1 = (_glowTime * 0.3f) % 1.0f;
        float vfxScale1 = baseScale * (1.0f + (progress1 * 0.5f));
        float vfxAlpha1 = (1.0f - progress1) * _currentAlpha;
        Color vfxColor1 = new Color(0.2f, 0.9f, 1.0f, vfxAlpha1);

        Vector2 size1 = texSize * vfxScale1;
        Rect2 vfxRect1 = new Rect2(-size1 / 2f, size1);
        DrawTextureRect(_glowTexture, vfxRect1, false, vfxColor1);

        float progress2 = ((_glowTime * 0.3f) + 0.5f) % 1.0f;
        float vfxScale2 = baseScale * (1.0f + (progress2 * 0.5f));
        float vfxAlpha2 = (1.0f - progress2) * _currentAlpha;
        Color vfxColor2 = new Color(0.2f, 0.9f, 1.0f, vfxAlpha2);

        Vector2 size2 = texSize * vfxScale2;
        Rect2 vfxRect2 = new Rect2(-size2 / 2f, size2);
        DrawTextureRect(_glowTexture, vfxRect2, false, vfxColor2);
    }
}
