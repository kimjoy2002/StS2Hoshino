using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class SupressionVeteranPower : StS2HoshinoPower, IBulletPowerInterface
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    
    
    private async void _internal_useBullet(PlayerChoiceContext choiceContext, CardModel card, Creature? applier, int amount) {        
        for(;amount > 0; amount--) {
            Flash();
            await CreatureCmd.GainBlock(base.Owner,  base.Amount, ValueProp.Unpowered, null);
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
