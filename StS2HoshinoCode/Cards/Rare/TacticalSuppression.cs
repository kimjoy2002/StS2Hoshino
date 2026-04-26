using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class TacticalSuppression() : StS2HoshinoCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllAllies)
{
    public override int AmmoCost { get; set; } = 4;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Arrival)
    ];
    protected override HashSet<CardTag> CanonicalTags => [
        StS2HoshinoCard.BulletCard
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => StS2Hoshino.StS2HoshinoCode.Utils.AmmoClass.GetInvadeCount(card.Owner)),
        new RepeatVar(4)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int amount = AmmoCost;
        if (amount > 0)
        {
            int bulletsUsed = 0;
            IReadOnlyList<Creature> enemies = base.CombatState.HittableEnemies;
            await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitCount(amount)
                .WithHitFx("vfx/vfx_heavy_blunt", sfx: "shotgunfireheavy.mp3".SfxPath())
                .BeforeDamage(() =>
                {
                    if (bulletsUsed > 0)
                    {
                        //총알 사용
                        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
                        foreach (IBulletPowerInterface item in enumerable)
                        {
                            item.UseBulletForMulti(choiceContext, this, enemies, base.Owner.Creature, 1);
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
                    item.UseBulletForMulti(choiceContext, this, enemies, base.Owner.Creature, 1);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(2m);
    }
}
