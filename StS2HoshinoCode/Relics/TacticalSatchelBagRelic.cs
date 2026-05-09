using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Saves.Runs;
using StS2Hoshino.StS2HoshinoCode.Hook;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Relics;

public class TacticalSatchelBagRelic : HoshinoBaseRelic, IOnBulletChanged
{
    private bool _isActivating;
    private int _counter;

    [SavedProperty]
    public int Counter
    {
        get => _counter;
        set
        {
            AssertMutable();
            _counter = value;
            UpdateDisplay();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("AmmoAmount", 7m),
        new PowerVar<ExpertPower>(1m)
    ];
    
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromPower<ExpertPower>()
    ];

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool ShowCounter => true;

    public override int DisplayAmount => _isActivating ? base.DynamicVars["AmmoAmount"].IntValue : (Counter % base.DynamicVars["AmmoAmount"].IntValue);

    private void UpdateDisplay()
    {
        if (_isActivating)
        {
            base.Status = RelicStatus.Normal;
        }
        else
        {
            base.Status = (Counter % base.DynamicVars["AmmoAmount"].IntValue == base.DynamicVars["AmmoAmount"].IntValue-1) ? RelicStatus.Active : RelicStatus.Normal;
        }
        InvokeDisplayAmountChanged();
    }

    public async Task OnBulletChanged(PlayerChoiceContext ctx, Player player, int before_bullet, int after_bullet)
    {
        if (base.Owner != player || _isActivating) return;

        // Only count when bullets decrease (ammo consumed)
        if (before_bullet > after_bullet)
        {
            int consumed = before_bullet - after_bullet;
            int nextValue = Counter + consumed;

            if (nextValue >= base.DynamicVars["AmmoAmount"].IntValue)
            {
                Counter = nextValue - base.DynamicVars["AmmoAmount"].IntValue;
                await DoActivate();
            }
            else
            {
                Counter = nextValue;
            }
        }
    }

    private async Task DoActivate()
    {
        _isActivating = true;
        Flash();
        UpdateDisplay();
        
        await PowerCmd.Apply<ExpertPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars["ExpertPower"].IntValue, base.Owner.Creature, null);
        
        await Cmd.Wait(0.5f);
        _isActivating = false;
        UpdateDisplay();
    }
}
