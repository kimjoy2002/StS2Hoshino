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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

public class IronWallSkill() : StS2HoshinoCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Shield),
        HoverTipFactory.FromPower<FrailPower>()
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(12, ValueProp.Move),
        new PowerVar<ShieldPower>(12m)
    
    ];
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (base.Owner.Creature.HasPower<FrailPower>())
        {
            await CommonActions.Apply<FrailPower>(base.Owner.Creature, this,
                -base.Owner.Creature.GetPowerAmount<FrailPower>());
        }
        await CommonActions.CardBlock(this, play);
        await CommonActions.ApplySelf<ShieldPower>(this);
    } 
    
    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(4m);
        DynamicVars["ShieldPower"].UpgradeValueBy(4m);
    }
}
