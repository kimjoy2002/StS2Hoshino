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

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class LockAndLoad() : StS2HoshinoCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("AddBullet", 2m)
    ];

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ReloadCmd.AddMaxAmmo(choiceContext,base.Owner, DynamicVars["AddBullet"].IntValue);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["AddBullet"].UpgradeValueBy(1m);
    }
}
