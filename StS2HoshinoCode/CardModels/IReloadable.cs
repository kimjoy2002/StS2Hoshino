namespace StS2Hoshino.StS2HoshinoCode.CardModels;
using MegaCrit.Sts2.Core.Entities.Players;

public interface IReloadable
{
    Task OnReload(Player player, int amount)
    {
        return Task.CompletedTask;
    }
}