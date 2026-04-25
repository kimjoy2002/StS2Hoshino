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


public sealed class BulletVunePower : StS2HoshinoPower, IOnReloaded, IBulletPowerInterface
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
    {
        if (base.Owner == player.Creature)
        {
            await PowerCmd.ModifyAmount(ctx, this, -Amount, null, null);
        }
    }

    public async void UseBullet(PlayerChoiceContext choiceContext, CardModel card, Creature? target, Creature? applier, int amount)
    {
        if (target != null)
        {
            Flash();
            await PowerCmd.Apply<VulnerablePower>(choiceContext, target, base.Amount, applier, card);
        }
    }
    
    public async void UseBulletForMulti(PlayerChoiceContext choiceContext, CardModel card, IEnumerable<Creature> targets, Creature? applier, int amount)
    {
        Flash();
        await PowerCmd.Apply<VulnerablePower>(choiceContext, targets, base.Amount, applier, card);
    }
}