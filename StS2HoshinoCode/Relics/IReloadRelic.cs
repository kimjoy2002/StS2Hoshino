using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.CardModels;
using MegaCrit.Sts2.Core.Entities.Players;

public interface IReloadRelic
{

    Task OnReload(PlayerChoiceContext choiceContext, Player reloader, int amount);
}