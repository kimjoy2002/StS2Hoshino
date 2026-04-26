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
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Basic;


[Pool(typeof(StS2HoshinoCardPool))]
public class FullBarrelFire() : StS2HoshinoCard(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    public override int AmmoCost { get; set; } = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");

        
        int prev = AmmoClass.GetCurrentAmmo(Owner);
        await AmmoClass.LoseAmmo(choiceContext,99, ((CardModel)this).Owner);
        int extraAmount = prev - AmmoClass.GetCurrentAmmo(Owner);
        int amount = extraAmount + AmmoCost;
        if (amount > 0)
        {
            int bulletsUsed = 0;
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target!)
                .WithHitCount(amount)
                .WithHitFx(sfx: "shotgunfirelight.mp3".SfxPath())
                .BeforeDamage(() =>
                {
                    if (bulletsUsed > 0)
                    {
                        //총알 사용
                        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
                        foreach (IBulletPowerInterface item in enumerable)
                        {
                            item.UseBullet(choiceContext, this, play.Target!, base.Owner.Creature, 1);
                        }
                    }
                    bulletsUsed++;
                    return Task.CompletedTask;
                })
                .Execute(choiceContext);

            if (bulletsUsed > 0)
            {
                IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
                foreach (IBulletPowerInterface item in enumerable)
                {
                    item.UseBullet(choiceContext, this, play.Target!, base.Owner.Creature, 1);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}