using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace StS2Hoshino.StS2HoshinoCode.Hook;

public interface IOnReloaded
{
    Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton);
}