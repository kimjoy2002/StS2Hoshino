using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class ShockTheWorldPower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _shieldBefore;

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner)
        {
            _shieldBefore = base.Owner.GetPower<ShieldPower>()?.Amount ?? 0;
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && props.IsPoweredAttack())
        {
            int shieldLoss = Math.Max(0, _shieldBefore - target.Block);
            
            if (shieldLoss > 0)
            {
                Flash();
                await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, base.Owner, shieldLoss * Amount, base.Owner, null);
            }
        }
    }
}
