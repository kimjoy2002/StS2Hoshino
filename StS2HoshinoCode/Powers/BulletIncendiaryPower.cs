using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class BulletIncendiaryPower : StS2HoshinoPower, IOnReloaded, IBulletPowerInterface
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
    {
        if (base.Owner == player.Creature)
        {
            await PowerCmd.ModifyAmount(this, -Amount, null, null);
        }
    }
    
    
    private async void _internal_useBullet(PlayerChoiceContext choiceContext, CardModel card, Creature? applier, int amount) {        
        for(;amount > 0; amount--) {
            Flash();

            var ownerCombatState = base.Owner.CombatState;
            if (ownerCombatState != null && ownerCombatState.CurrentSide != base.Owner.Side)
            {
                return;
            }
            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                NFireBurstVfx? child = NFireBurstVfx.Create(hittableEnemy, 0.75f);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
            }
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.Amount, ValueProp.Unpowered, base.Owner, null);
        }
    }

    public async void UseBullet(PlayerChoiceContext choiceContext, CardModel card, Creature? target, Creature? applier, int amount)
    {
        _internal_useBullet(choiceContext, card, applier, amount);
    }
    
    public async void UseBulletForMulti(PlayerChoiceContext choiceContext, CardModel card, IEnumerable<Creature> targets, Creature? applier, int amount)
    {
        _internal_useBullet(choiceContext, card, applier, amount);
    }
}