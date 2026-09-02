using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using StS2Hoshino.StS2HoshinoCode.Hook;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Relics;

public sealed class AnywhereTicketRelic : HoshinoBaseRelic, IOnBulletChanged
{
    private bool _dexterityApplied;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    private bool DexterityApplied
    {
        get => _dexterityApplied;
        set
        {
            AssertMutable();
            _dexterityApplied = value;
        }
    }

    public override Task BeforeCombatStart()
    {
        return UpdateDexterity(AmmoClass.GetCurrentAmmo(Owner));
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        DexterityApplied = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public Task OnBulletChanged(PlayerChoiceContext choiceContext, Player player, int beforeBullet, int afterBullet)
    {
        return player == Owner ? UpdateDexterity(afterBullet) : Task.CompletedTask;
    }

    private async Task UpdateDexterity(int ammo)
    {
        bool shouldApply = ammo == 0;
        Status = shouldApply ? RelicStatus.Active : RelicStatus.Normal;
        if (shouldApply == DexterityApplied)
        {
            return;
        }

        Flash();
        decimal amount = DynamicVars.Dexterity.BaseValue * (shouldApply ? 1m : -1m);
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, amount, Owner.Creature, null);
        DexterityApplied = shouldApply;
    }
}
