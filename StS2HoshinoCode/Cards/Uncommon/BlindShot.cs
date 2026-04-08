using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

public class BlindShot() : StS2HoshinoCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    public override int AmmoCost => 1;
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new RepeatVar(3)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int count_ = base.DynamicVars.Repeat.IntValue;
        for (int i = 0 ; i < count_; i++)
        {
            List<Creature> validTargets = base.CombatState.HittableEnemies.Where<Creature>((Func<Creature, bool>) (c => c.IsAlive)).ToList<Creature>();
            if (validTargets.Count > 0)
            {
                Creature singleTarget = RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) validTargets);
                if (singleTarget != null)
                {
                    await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(singleTarget)
                        .Execute(choiceContext);
            
                    //총알 사용
                    IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
                    foreach (IBulletPowerInterface item in enumerable)
                    {
                        item.UseBullet(this, singleTarget,base.Owner.Creature, 0);
                    }
                }
            }
        }
        IEnumerable<IBulletPowerInterface> enumerable2 = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable2)
        {
            item.UseBullet(this, null, base.Owner.Creature, 1);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
