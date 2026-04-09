using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;


public sealed class BulletIncendiaryPower : StS2HoshinoPower, IOnReloaded, IBulletPowerInterface
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
    {
        await PowerCmd.ModifyAmount(this, -Amount, null, null);
    }
    
    
    private async void _internal_useBullet(CardModel card, Creature? applier, int amount) {        
        for(;amount > 0; amount--) {
            Flash();
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_heavy_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);
        }
    }

    public async void UseBullet(CardModel card, Creature? target, Creature? applier, int amount)
    {
        _internal_useBullet(card, applier, amount);
    }
    
    public async void UseBulletForMulti(CardModel card, IEnumerable<Creature> targets, Creature? applier, int amount)
    {
        _internal_useBullet(card, applier, amount);
    }
}