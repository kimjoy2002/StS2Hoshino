using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.Hook;

public interface IOnInvaded
{
    Task OnInvaded(PlayerChoiceContext choiceContext, Player player, CardModel card);
}