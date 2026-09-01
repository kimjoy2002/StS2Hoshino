using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class TacticalReloadPower : StS2HoshinoPower, IOnReloaded
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    
    public async Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
    {
        if (base.Owner == player.Creature)
        {
            Flash();
            await Cmd.CustomScaledWait(0.1f, 0.2f);

            var combatState = base.Owner.CombatState;
            if (combatState == null)
            {
                return;
            }

            foreach (Creature enemy in combatState.HittableEnemies)
            {
                NFireBurstVfx? fireVfx = NFireBurstVfx.Create(enemy, 0.75f);
                if (fireVfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(fireVfx);
                }
            }

            await CreatureCmd.Damage(ctx, combatState.HittableEnemies, Amount, ValueProp.Unpowered, base.Owner);
        }
    }
}
