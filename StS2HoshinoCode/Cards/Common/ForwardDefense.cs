using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

public class ForwardDefense() : StS2HoshinoCard(2, CardType.Skill, CardRarity.Common, TargetType.Self), IInvade
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(11, ValueProp.Move),
        new BlockVar("InvadeBlock", 3m, ValueProp.Move)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }
    
    public async Task OnInvade(PlayerChoiceContext choiceContext, CardModel card)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars["InvadeBlock"], null);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(3m);
        DynamicVars["InvadeBlock"].UpgradeValueBy(1m);
    }
}
