using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.Hook;

public interface IOnCardReturnedToDrawPile
{
    Task OnCardReturnedToDrawPile(PlayerChoiceContext ctx, Player player, CardModel card);
}
