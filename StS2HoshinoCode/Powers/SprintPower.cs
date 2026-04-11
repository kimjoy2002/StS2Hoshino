using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class SprintPower : StS2HoshinoPower, IOnBulletChanged
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    
    public async Task OnBulletChanged(PlayerChoiceContext ctx, Player player, int before_bullet, int after_bullet)
    {
        if (base.Owner == player.Creature)
        {
            if (after_bullet == 0 && before_bullet > 0)
            {
                Flash();
                await CardPileCmd.Draw(ctx, base.Amount, player);
            }
        }
    }
}
