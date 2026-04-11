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
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class RoundManualChambering() : StS2HoshinoCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Reload)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RoundManualChamberingPower>(1m),
        new PowerVar<FreeReloadPower>(1m)
    ];


    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int get_free_reload = 1 - AmmoClass.getReloadCount(base.Owner);
        if (base.Owner.Creature.HasPower<RoundManualChamberingPower>())
        {
            get_free_reload += base.Owner.Creature.GetPower<RoundManualChamberingPower>()!.Amount;
        }
        await CommonActions.ApplySelf<RoundManualChamberingPower>(this);
        if (get_free_reload > 0)
        {
            await CommonActions.ApplySelf<FreeReloadPower>(this);
        }
        
    }
    
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
