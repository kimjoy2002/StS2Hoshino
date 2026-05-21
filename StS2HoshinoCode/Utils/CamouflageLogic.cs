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
        List<CardModel> list = CardFactory.GetForCombat(player,
            from c in player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            where !(c is IInvade) && c.CanBeGeneratedInCombat
            select c,
            1,
            player.RunState.Rng.CombatCardGeneration).ToList();

        if (list.Count == 0)
        {
            return;
        }

        var newCard = list[0];

        if (card.IsUpgraded)
        {
            CardCmd.Upgrade(newCard);
        }

        if (newCard.Tags is HashSet<CardTag> tagSet)
        {
            tagSet.Add(StS2HoshinoCard.CamouflageTag);
        }

        await CardCmd.Afflict<CamouflagedAffliction>(newCard, 1);

        await CardCmd.Transform(card, newCard);
    }
}
