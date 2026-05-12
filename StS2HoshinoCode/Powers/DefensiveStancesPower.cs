using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class DefensiveStancesPower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != null && base.Owner != target && target.Side == base.Owner.Side)
        {
            return 0m;
        }
        return 1m;
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        this.Removed += () =>
        {
            if (base.Owner != null)
            {
                HoshinoVisualUtils.RemoveBarrierVisual(base.Owner);
            }
        };
        if (base.Owner.IsPlayer && base.Owner.Player != null)
        {
            HoshinoVisualUtils.ApplyBarrierVisual(base.Owner);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await base.AfterRemoved(oldOwner);
        if (oldOwner.IsPlayer && oldOwner.Player != null)
        {
            HoshinoVisualUtils.RemoveBarrierVisual(oldOwner);
        }
    }
}
