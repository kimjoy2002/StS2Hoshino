using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace StS2Hoshino.StS2HoshinoCode.CardModels;
using MegaCrit.Sts2.Core.Entities.Players;

public interface IRunout
{
    Task OnRunout(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
}