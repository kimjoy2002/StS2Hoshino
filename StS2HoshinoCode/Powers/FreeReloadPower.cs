using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class FreeReloadPower : StS2HoshinoPower, IOnReloaded
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
    {
        if (useButton)
        {
            await PowerCmd.ModifyAmount(this, -1, null, null);
        }
    }
}
