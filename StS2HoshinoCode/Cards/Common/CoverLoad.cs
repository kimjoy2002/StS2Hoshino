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
using StS2Hoshino.StS2HoshinoCode.Core;
using StS2Hoshino.StS2HoshinoCode.Keywords;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

public class CoverLoad() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override int AmmoCost => 0;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Reload)
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int max = Utils.AmmoClass.GetMaxAmmo(base.Owner);
        int current = Utils.AmmoClass.GetCurrentAmmo(base.Owner);
        int empty = max - current;
        if (empty > 0)
        {
            for (;empty>0;empty--) {
                await CommonActions.CardBlock(this, play);
            }
            //await BlockCmd.Block((int)(base.DynamicVars.Block.BaseValue * empty)).FromCard(this).Targeting(base.Owner.Creature).Execute(choiceContext);
        }
        await ReloadCmd.Execute(choiceContext, base.Owner);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(1m);
    }
}
