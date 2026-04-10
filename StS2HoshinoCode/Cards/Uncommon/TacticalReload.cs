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
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

public class TacticalReload() : StS2HoshinoCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<TacticalReloadPower>(4m)
    ];


    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.ApplySelf<TacticalReloadPower>(this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["TacticalReloadPower"].UpgradeValueBy(2m);
    }
}
