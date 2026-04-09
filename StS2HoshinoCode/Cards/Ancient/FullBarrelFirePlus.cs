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


public class FullBarrelFire() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override int AmmoCost { get; set; } = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        
        Creature singleTarget = play.Target;
        int extraAmount = AmmoClass.LoseAmmo(99, ((CardModel)this).Owner);
        int amount = extraAmount + AmmoCost;
        for (; amount > 0; amount--)
        {
            if (!singleTarget.IsAlive) {
                List<Creature> validTargets = base.CombatState.HittableEnemies.Where<Creature>((Func<Creature, bool>) (c => c.IsAlive)).ToList<Creature>();
                if (validTargets.Count > 0)
                {
                    singleTarget = RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) validTargets);
                    if (singleTarget == null)
                    {
                        break;
                    }
                }
            }
            
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(singleTarget).Execute(choiceContext);

            //총알 사용
            IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
            foreach (IBulletPowerInterface item in enumerable)
            {
                item.UseBullet(this, play.Target,base.Owner.Creature, 1);
            }
            
            if(!singleTarget.IsAlive) {
                await ReloadCmd.Execute(choiceContext, base.Owner);
                amount = AmmoClass.LoseAmmo(99, ((CardModel)this).Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}