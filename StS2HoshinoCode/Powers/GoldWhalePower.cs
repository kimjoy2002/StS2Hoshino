using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using StS2Hoshino.StS2HoshinoCode.Cards.Special;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class GoldWhalePower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Owner.Player != null)
        {
            Flash();
            var player = base.Owner.Player;

            for (int i = 0; i < base.Amount; i++)
            {
                room.AddExtraReward(player, new RelicReward(player));
                await Karma.AddToDeckAndCombine(player, 1);
            }
        }
    }
}
