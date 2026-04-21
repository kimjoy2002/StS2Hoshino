using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class Revenge() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new DynamicVar("Replay", 1m)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.ShuffleIfNecessary(choiceContext, base.Owner);
        List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
            orderby c.Rarity, c.Id
            select c).ToList();
        List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 1))).ToList();
        bool success_ = false;
        foreach (CardModel item in list)
        {
            if (PileType.Draw.GetPile(base.Owner).Cards[0].GetType() == item.GetType())
            {
                item.BaseReplayCount +=  base.DynamicVars["Replay"].IntValue;
                break;
            }
        }
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Replay"].UpgradeValueBy(1m);
    }
}
