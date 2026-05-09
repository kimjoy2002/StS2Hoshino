using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
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
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class Camouflage() : StS2HoshinoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IInvade
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Arrival)
    ];
    protected override HashSet<CardTag> CanonicalTags => [CamouflageTag];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    public async Task OnInvade(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        if (card != this)
        {
            return;
        }
        await CamouflageLogic.Transform(choiceContext, player, card);
    }

    protected override void OnUpgrade()
    {
    }
}
