using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Core;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Hook;
using StS2Hoshino.StS2HoshinoCode.Utils;

public static class ReloadCmd
{
    public static event Func<Player, int, Task>? Reloaded;

    private static string ReloadSfxPath => "reload.mp3".SfxPath();
    public static async Task Execute(PlayerChoiceContext choiceContext, Player player, int amount = -1, bool isButton = false)
    {
        AmmoClass.SetAmmo(amount ==-1?AmmoClass.GetMaxAmmo(player):amount, true, player);

        await AmmoClass.ProcessPendingTriggers(choiceContext);
        
        SfxCmd.Play(ReloadSfxPath);
        await NotifyPowers(player, amount);
        await NotifyCards(player, amount);
        await HoshinoHook.OnReload(choiceContext, player, isButton);
    }

    
    public static async Task RemoveMaxAmmo(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        if (AmmoClass.GetMaxAmmo(player) > 0)
        {
            AmmoClass.SetMaxAmmo(player, AmmoClass.GetMaxAmmo(player) - amount);

            await AmmoClass.ProcessPendingTriggers(choiceContext);
        }
    }
    public static async Task AddMaxAmmo(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        AmmoClass.SetMaxAmmo(player, AmmoClass.GetMaxAmmo(player) + amount);

        await AmmoClass.ProcessPendingTriggers(choiceContext);
    }

    private static async Task NotifyPowers(Player player, int amount)
    {
        if (Reloaded != null) await Reloaded.Invoke(player, amount);
    }

    private static async Task NotifyCards(Player? player, int amount)
    {
        if (player?.PlayerCombatState?.AllPiles != null)
            foreach (var pile in player.PlayerCombatState?.AllPiles!)
            {
                var reloadCards = pile.Cards.OfType<IReloadable>().ToList();
                foreach (var card in reloadCards) await card.OnReload(player, amount);
            }
    }
}