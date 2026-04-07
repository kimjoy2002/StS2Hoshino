using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Basic;


public class FullBarrelFire() : StS2HoshinoCard(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    public override int AmmoCost { get; set; } = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int extraAmount = AmmoClass.LoseAmmo(99, ((CardModel)this).Owner);
        int amount = extraAmount + AmmoCost;
        await AmmoClass.ProcessPendingTriggers(choiceContext);

        if (amount > 0)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(amount).FromCard(this)
                .Targeting(play.Target)
                // .WithHitFx("vfx/vfx_starry_impact", null, "blunt_attack.mp3")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}