using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Basic;


public class FullBarrelFire() : StS2HoshinoCard(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];
    protected override bool IsPlayable => AmmoClass.hasAmmo(1, ((CardModel)this).Owner);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int amount = AmmoClass.LoseAmmo(99, ((CardModel)this).Owner);
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