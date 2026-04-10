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

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

public class Retry() : StS2HoshinoCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move),
        new CardsVar(3)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        int num = Math.Min(base.DynamicVars.Cards.IntValue, PileType.Discard.GetPile(base.Owner).Cards.Count);
        await CardPileCmd.Add(await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Discard.GetPile(base.Owner).Cards, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, base.DynamicVars.Cards.IntValue)), PileType.Draw);
        for (; num > 0; num--)
        {
            await CommonActions.CardBlock(this, play);
        }
        await OnlyDeckShuffle(choiceContext, base.Owner);
    }
    
    
    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(2m);
    }
}
