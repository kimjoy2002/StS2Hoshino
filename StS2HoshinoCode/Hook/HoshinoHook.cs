

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.Hook;


public class HoshinoHook
{
    private static async Task Dispatch<T>(PlayerChoiceContext ctx, Player player, Func<T, Task> invoke)
        where T : class
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            ctx.PushModel(abstractModel);
            await invoke(model);
            ctx.PopModel(abstractModel);
        }
    }

    public static Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
        => Dispatch<IOnReloaded>(ctx, player, m => m.OnReload(ctx, player, useButton));
    
    
    public static Task OnBulletChanged(PlayerChoiceContext ctx, Player player, int before_bullet, int after_bullet)
        => Dispatch<IOnBulletChanged>(ctx, player, m => m.OnBulletChanged(ctx, player, before_bullet, after_bullet));
    
    
    public static Task OnInvaded(PlayerChoiceContext ctx, Player player, CardModel card)
        => Dispatch<IOnInvaded>(ctx, player, m => m.OnInvaded(ctx,player,card));

    
}