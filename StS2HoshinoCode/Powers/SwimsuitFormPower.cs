using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;



public sealed class SwimsuitFormPower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, base.Amount);
        List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(player).Cards
            orderby c.Rarity, c.Id
            select c).ToList();
        List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, player, prefs)).ToList();
        foreach (CardModel item in list)
        {
            await CardPileCmd.Add(item, PileType.Hand);
            item.SetToFreeThisTurn();
        }
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        await CreatureCmd.TriggerAnim(base.Owner, "Idle", 0f);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        await base.AfterRemoved(oldOwner);
        await CreatureCmd.TriggerAnim(oldOwner, "Idle", 0f);
    }
}
