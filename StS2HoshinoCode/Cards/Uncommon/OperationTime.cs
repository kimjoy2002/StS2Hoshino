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
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

public class OperationTime() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(3),
        new DynamicVar("PutBack", 1m)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        CardModel[] array = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, base.DynamicVars["PutBack"].IntValue), context: choiceContext, player: base.Owner, filter: null, source: this)).ToArray();
        if (array.Length != 0)
        {
            await CardPileCmd.Add(array, PileType.Draw, CardPilePosition.Top);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
