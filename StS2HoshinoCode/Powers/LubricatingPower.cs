using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using StS2Hoshino.StS2HoshinoCode.Hook;
using StS2Hoshino.StS2HoshinoCode.Rewards;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class LubricatingPower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool IsInstanced => true;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Amount > 0)
        {
            var ownerPlayer = base.Owner.Player;
            if (ownerPlayer != null)
                room.AddExtraReward(ownerPlayer, new CardEnchantReward(ownerPlayer, base.Amount));
        }
        return Task.CompletedTask;
    }

}
