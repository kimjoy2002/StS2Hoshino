using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Random;
using System.Collections.Generic;
using StS2Hoshino.StS2HoshinoCode.Models.Afflictions;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using StS2Hoshino.StS2HoshinoCode.CardModels;

namespace StS2Hoshino.StS2HoshinoCode.Utils;

public static class CamouflageLogic
{
    public static async Task Transform(PlayerChoiceContext ctx, Player player, CardModel card)
    {
        List<CardModel> list = CardFactory.GetForCombat(player, from c in player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            where !(c is IInvade) && c.CanBeGeneratedInCombat
            select c, 1, player.RunState.Rng.CombatCardGeneration).ToList();

        if (list.Count == 0)
        {
            return;
        }
        //
        // var pool = ModelDb.CardPool<StS2HoshinoCardPool>();
        //
        // var validCardIds = pool.AllCardIds
        //     .Where(id => {
        //         var model = ModelDb.GetById<CardModel>(id);
        //         return !(model is IInvade) && model.CanBeGeneratedInCombat;
        //     })
        //     .ToList();

        var randomCardId = list[0].Id;
        
        var canonical = ModelDb.GetById<CardModel>(randomCardId);
        var newCard = (player.Creature.CombatState != null) 
            ? player.Creature.CombatState.CreateCard(canonical, player) 
            : canonical.ToMutable();
        
        if (card.IsUpgraded)
        {
            newCard.UpgradeInternal();
            newCard.FinalizeUpgradeInternal();
        }
        
        if (newCard.Tags is HashSet<CardTag> tagSet)
        {
            tagSet.Add(StS2HoshinoCard.CamouflageTag);
        }
        else
        {
            var newTags = new HashSet<CardTag>(newCard.Tags);
            newTags.Add(StS2HoshinoCard.CamouflageTag);
        }

        await CardCmd.Afflict<CamouflagedAffliction>(newCard, 1);
        
        await CardCmd.Transform(card, newCard);
    }
}
