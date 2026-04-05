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
using StS2Hoshino.StS2HoshinoCode.Core;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

public class NoWasteReload() : StS2HoshinoCard(0, CardType.Skill, CardRarity.Common, TargetType.Self), IRunout
{
    public override int AmmoCost => 0;
    protected override HashSet<CardTag> CanonicalTags => [
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ReloadCmd.Execute(choiceContext, base.Owner);
    }
    
    public async Task OnRunout(PlayerChoiceContext choiceContext, CardPlay play)
    {
        StS2HoshinoMain.Logger.Info("[NoWasteReload] OnRunout");
        await PlayerCmd.GainEnergy(2, base.Owner);
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
