using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Cards.Common;
using StS2Hoshino.StS2HoshinoCode.Cards.Special;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class PrepareSuppression() : StS2HoshinoCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromCard<FinishAttack>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PrepareSuppressionPower>(12m)
    ];


    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.ApplySelf<PrepareSuppressionPower>(this);
        
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["PrepareSuppressionPower"].UpgradeValueBy(4m);
    }
}
