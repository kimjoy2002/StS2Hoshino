using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

[Pool(typeof(StS2HoshinoCardPool))]
public class ShotgunShuffle() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{	

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(7, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
        CardPile pile = PileType.Discard.GetPile(base.Owner);
        CardModel? cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, base.Owner, prefs)).FirstOrDefault();
        if (cardModel != null)
        {
            await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Random);
        }
        await OnlyDeckShuffle(choiceContext, base.Owner);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
    }
    
    
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }

}
