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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class Bulletproof() : StS2HoshinoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IInvade
{    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Shield)
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move),
        new PowerVar<ShieldPower>(4m),
        new IntVar("Repeat", 1m),
        new EnergyVar(1)
        
    
    ];
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await CommonActions.ApplySelf<ShieldPower>(choiceContext, this);
    } 
    
    public Task OnInvade(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }
        BaseReplayCount += base.DynamicVars["Repeat"].IntValue;
        base.EnergyCost.AddThisCombat(1);
        return Task.CompletedTask;
    }

    
    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(1m);
        DynamicVars["ShieldPower"].UpgradeValueBy(1m);
    }
}
