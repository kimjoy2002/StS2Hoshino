using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Cards;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class BlackMarketPower : StS2HoshinoPower, IOnReloaded
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    
    public async Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
    {
        if (base.Owner == player.Creature)
        {
            Flash();
            List<CardModel> list = CardFactory.GetForCombat(base.Owner.Player, from c in base.Owner.Player.Character.CardPool.GetUnlockedCards(base.Owner.Player.UnlockState, base.Owner.Player.RunState.CardMultiplayerConstraint)
                where c.Tags.Contains(StS2HoshinoCard.BulletCard)
                select c, base.Amount, base.Owner.Player.RunState.Rng.CombatCardGeneration).ToList();
            foreach (CardModel item in list)
            {
                await CardPileCmd.AddGeneratedCardToCombat(item, PileType.Hand, player);
            }
        }
    }
}
