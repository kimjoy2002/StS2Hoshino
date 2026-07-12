using BaseLib.Config;
using Godot;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Config;

[ConfigHoverTipsByDefault]
public class HoshinoModConfig : SimpleModConfig
{
    private const int PreviewDelayMs = 180;
    private static float _lastBulletAttackVolumePercent;
    private static float _lastReloadVolumePercent;
    private static CancellationTokenSource? _bulletPreviewToken;
    private static CancellationTokenSource? _reloadPreviewToken;

    public HoshinoModConfig()
    {
        _lastBulletAttackVolumePercent = BulletAttackVolumePercent;
        _lastReloadVolumePercent = ReloadVolumePercent;
        ConfigChanged += OnConfigChanged;
    }

    public enum ReloadButtonPosition
    {
        Up,
        Down,
        Left,
        Right
    };

    [ConfigSection("ReloadSettings")]
    [ConfigHoverTip]
    public static bool IsVewingReloadButton { get; set; } = true;

    [ConfigVisibleIf(nameof(IsVewingReloadButton))]
    [ConfigHoverTip]
    public static ReloadButtonPosition ButtonPosition { get; set; } = ReloadButtonPosition.Up;

    [ConfigSection("AudioSettings")]
    [ConfigSlider(0.0, 300.0, 10.0, Format = "{0:0}%")]
    [ConfigHoverTip]
    public static float BulletAttackVolumePercent { get; set; } = 100.0f;

    [ConfigSlider(0.0, 300.0, 10.0, Format = "{0:0}%")]
    [ConfigHoverTip]
    public static float ReloadVolumePercent { get; set; } = 100.0f;

    public override void SetupConfigUI(Control optionContainer)
    {
        _lastBulletAttackVolumePercent = BulletAttackVolumePercent;
        _lastReloadVolumePercent = ReloadVolumePercent;
        base.SetupConfigUI(optionContainer);
    }

    private static void OnConfigChanged(object? sender, EventArgs e)
    {
        if (!Mathf.IsEqualApprox(_lastBulletAttackVolumePercent, BulletAttackVolumePercent))
        {
            _lastBulletAttackVolumePercent = BulletAttackVolumePercent;
            _bulletPreviewToken = SchedulePreview(_bulletPreviewToken, "shotgunfire.mp3".SfxPath());
        }

        if (!Mathf.IsEqualApprox(_lastReloadVolumePercent, ReloadVolumePercent))
        {
            _lastReloadVolumePercent = ReloadVolumePercent;
            _reloadPreviewToken = SchedulePreview(_reloadPreviewToken, "reload.mp3".SfxPath());
        }
    }

    private static CancellationTokenSource SchedulePreview(CancellationTokenSource? previousToken, string sfxPath)
    {
        previousToken?.Cancel();
        previousToken?.Dispose();

        CancellationTokenSource tokenSource = new();
        PlayPreviewAfterDelay(tokenSource, sfxPath);
        return tokenSource;
    }

    private static async void PlayPreviewAfterDelay(CancellationTokenSource tokenSource, string sfxPath)
    {
        try
        {
            await Task.Delay(PreviewDelayMs, tokenSource.Token);
            StS2HoshinoMain.PlaySfx(sfxPath);
        }
        catch (OperationCanceledException)
        {
        }
    }
}
