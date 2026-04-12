using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class DefensiveStancesPower : StS2HoshinoPower, IOnInvaded
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnInvaded(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        if (base.Owner == player.Creature)
        {
            Flash();
            await CreatureCmd.GainBlock(player.Creature,  base.Amount, ValueProp.Unpowered, null);
            await PowerCmd.Apply<ShieldPower>(base.Owner, Amount, base.Owner, null);
        }
    }
}
