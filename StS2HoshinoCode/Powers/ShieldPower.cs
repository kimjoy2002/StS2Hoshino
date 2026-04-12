using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class ShieldPower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal _, Creature? __, CardModel? cardSource)
    {
        if (power == this)
        {
            if (base.Owner.Block < Amount)
            {
                await PowerCmd.ModifyAmount(this, -(Amount - base.Owner.Block), null, null);
            }
        }
    }
    
    
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target == base.Owner)
        {
            if (target.Block < Amount)
            {
                await PowerCmd.ModifyAmount(this, -(Amount - target.Block), null, null);
            }
        }
    }

    public override async Task AfterBlockGained(
        Creature creature,
        Decimal amount,
        ValueProp props,
        CardModel? cardSource)
    {
        if (creature == base.Owner) {
            if (creature.Block < Amount)
            {
                await PowerCmd.ModifyAmount(this, -(Amount - creature.Block), null, null);
            }
        }
    }

    public override bool ShouldClearBlock(Creature creature)
    {
        if (creature != base.Owner)
        {
            return true;
        }
        return false;
    }

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != base.Owner)
        {
            return;
        }
        int block = creature.Block;
        if (block != 0)
        {
            if (block > Amount)
            {
                await CreatureCmd.LoseBlock(creature, block - Amount);
            }
            Flash();
        }
    }
}