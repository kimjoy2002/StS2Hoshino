using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class GoldWhalePower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Owner.Player != null)
        {
            Flash();
            var player = base.Owner.Player;

            List<CardModel> curses = new();
            for (int i = 0; i < base.Amount; i++)
            {
                room.AddExtraReward(player, new RelicReward(player));

                if (player.RunState.Rng.CombatCardSelection.NextInt(100) < 33)
                {
                    IEnumerable<CardModel> filteredCurses = (from c in ModelDb.CardPool<CurseCardPool>()
                            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                        where c.CanBeGeneratedByModifiers
                        select c);
                        
                    //IEnumerable<CardModel> filteredCurses2 = cursePool.AllCards.Where(c => c.Id != ModelDb.Card<AscendersBane>().Id).ToList();


                    var cardModels = filteredCurses as CardModel[] ?? filteredCurses.ToArray();
                    if (cardModels.Any())
                    {
                        CardModel? randomCurseModel = player.RunState.Rng.CombatCardSelection.NextItem(cardModels);
                        if (randomCurseModel != null)
                        {
                            curses.Add(randomCurseModel);
                            StS2HoshinoMain.Logger.Info($"[GoldWhalePower] add GoldWale {randomCurseModel.Id}");
                        }
                    }
                }
            }

            if (curses.Count > 0)
            {
                StS2HoshinoMain.Logger.Info($"[GoldWhalePower] add AddCursesToDeck {curses}");
                await CardPileCmd.AddCursesToDeck(curses, player);
            }
        }
    }
}
