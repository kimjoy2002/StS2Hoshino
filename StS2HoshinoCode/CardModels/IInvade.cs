using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.CardModels;
using MegaCrit.Sts2.Core.Entities.Players;

public interface IInvade
{
    Task OnInvade(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        return Task.CompletedTask;
    }
}