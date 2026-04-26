using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class FullBarrelFirePlus() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override int AmmoCost { get; set; } = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Reload)
        
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        
        Creature currentTarget = play.Target;
        
        int prevAmmo = AmmoClass.GetCurrentAmmo(Owner);
        await AmmoClass.LoseAmmo(choiceContext, 99, ((CardModel)this).Owner);
        int consumedAmmo = prevAmmo - AmmoClass.GetCurrentAmmo(Owner);
        int amount = consumedAmmo + AmmoCost;

        while (true)
        {
            if (amount > 0)
            {
                int bulletsUsed = 0;
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(currentTarget)
                    .WithHitCount(amount)
                    .WithHitFx(sfx: "shotgunfireheavy.mp3".SfxPath())
                    .BeforeDamage(() =>
                    {
                        if (bulletsUsed > 0)
                        {
                            //총알 사용
                            IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
                            foreach (IBulletPowerInterface item in enumerable)
                            {
                                item.UseBullet(choiceContext, this, currentTarget!, base.Owner.Creature, 1);
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
                        item.UseBullet(choiceContext, this, currentTarget!, base.Owner.Creature, 1);
                    }
                }
            }

            if (!currentTarget.IsAlive)
            {
                List<Creature> validTargets = base.CombatState!.HittableEnemies.Where(c => c.IsAlive).ToList();
                if (validTargets.Count > 0)
                {
                    currentTarget = RunState!.Rng.CombatTargets.NextItem(validTargets);
                    if (currentTarget != null)
                    {
                        await ReloadCmd.Execute(choiceContext, base.Owner);
                        
                        int tempAmmo = AmmoClass.GetCurrentAmmo(Owner);
                        await AmmoClass.LoseAmmo(choiceContext, 99, ((CardModel)this).Owner);
                        amount = tempAmmo - AmmoClass.GetCurrentAmmo(Owner);
                        continue;
                    }
                }
            }
            break;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}