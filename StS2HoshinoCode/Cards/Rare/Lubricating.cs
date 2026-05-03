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
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class Lubricating() : StS2HoshinoCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    // protected override IEnumerable<IHoverTip> ExtraHoverTips
    // {
    //     get
    //     {
    //         List<IHoverTip> list = new List<IHoverTip>();
    //         list.AddRange(HoverTipFactory.FromEnchantment<Sharp>());
    //         return new List<IHoverTip>(list);
    //     }
    // }

    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromEnchantment<Sharp>(base.DynamicVars["LubricatingPower"].IntValue)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<LubricatingPower>(2m)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<LubricatingPower>(choiceContext, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["LubricatingPower"].UpgradeValueBy(1m);
    }
}
