using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace StS2Hoshino.StS2HoshinoCode.Hook;

public interface IOnBulletChanged
{
    Task OnBulletChanged(PlayerChoiceContext ctx, Player player, int before_bullet, int after_bullet);
}