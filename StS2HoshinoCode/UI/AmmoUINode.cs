using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.UI;

public partial class AmmoUINode : Control
{
    private Player? _player;

    private int _max = 4;
    private int _current = 4;

    private const float AmmoSpacing = 20f;   // 탄환 사이 간격
    private const float DrawScale = 1.0f;    // 필요하면 1.5f, 2.0f 등으로 조절

    private static string BulletEmptyPath => "res://StS2Hoshino/images/charui/bullet_empty.png";
    private static string BulletFullPath  => "res://StS2Hoshino/images/charui/bullet_full.png";

    private Texture2D? _ammoEmptyTexture;
    private Texture2D? _ammoFullTexture;

    public static AmmoUINode Create(Player player)
    {
        AmmoUINode obj = new AmmoUINode
        {
            _player = player
        };

        obj._ammoEmptyTexture = GD.Load<Texture2D>(BulletEmptyPath);
        obj._ammoFullTexture = GD.Load<Texture2D>(BulletFullPath);

        obj.UpdateLayoutSize();
        obj.QueueRedraw();
        return obj;
    }

    public override void _Ready()
    {
        UpdateLayoutSize();
        QueueRedraw();
        AmmoClass.OnChanged += HandleAmmoChanged;
    }

    public override void _ExitTree()
    {
        AmmoClass.OnChanged -= HandleAmmoChanged;
    }
    public void SetAmmo(int current, int max)
    {
        _current = Mathf.Clamp(current, 0, max);
        _max = Mathf.Max(0, max);
        UpdateLayoutSize();
        QueueRedraw();
    }

    
    private void HandleAmmoChanged(int current, int max)
    {
        StS2HoshinoMain.Logger.Info($"handle_ammo_changed {current}/{_max}");
        if (_player != null && AmmoClass.CurrentAmmoGainer == _player)
        {
            StS2HoshinoMain.Logger.Info($"handle_ammo_changed clear {current}/{_max}");
            SetAmmo(current, max);
        }
    }
    
    private void UpdateLayoutSize()
    {
        Texture2D? tex = _ammoFullTexture ?? _ammoEmptyTexture;
        if (tex == null || _max <= 0)
            return;

        Vector2 bulletSize = tex.GetSize() * DrawScale;

        float totalWidth = bulletSize.X + (_max - 1) * AmmoSpacing;
        float totalHeight = bulletSize.Y;

        CustomMinimumSize = new Vector2(totalWidth, totalHeight);
        Size = CustomMinimumSize;
    }

    private float GetAmmoDrawX(int index, float bulletWidth)
    {
        float centerOffset = index - (_max - 1) * 0.5f;
        return Size.X * 0.5f + centerOffset * AmmoSpacing - bulletWidth * 0.5f;
    }
    private float GetAmmoDrawY(float bulletHeight)
    {
        return Size.Y - bulletHeight - 350.0f;
    }

    public override void _Draw()
    {
        if (_player == null || _ammoEmptyTexture == null || _ammoFullTexture == null || _max <= 0)
            return;

        Vector2 bulletSize = _ammoFullTexture.GetSize() * DrawScale;
        float y = GetAmmoDrawY(bulletSize.Y);

        for (int i = 0; i < _max; i++)
        {
            Texture2D tex = i < _current ? _ammoFullTexture : _ammoEmptyTexture;
            float x = GetAmmoDrawX(i, bulletSize.X);

            DrawTextureRect(
                tex,
                new Rect2(x, y, bulletSize.X, bulletSize.Y),
                false
            );
        }
    }
}