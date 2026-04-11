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
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class SnapShot() : StS2HoshinoCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override int AmmoCost { get; set; } = 0;
    protected override bool HasEnergyCostX => true;
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Reload)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];

    protected override bool IsPlayable => base.IsPlayable && AmmoClass.GetMaxAmmo(Owner) > 0;
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");

        if (AmmoClass.GetMaxAmmo(Owner) <= 0)
            return;
        int amount = ResolveEnergyXValue();
        for (; amount > 0; amount--)
        {
            if (play.Target.IsAlive)
            {
                if (Utils.AmmoClass.isEmptyAmmo(base.Owner))
                {
                    await ReloadCmd.Execute(choiceContext, base.Owner);
                }
                Utils.AmmoClass.LoseAmmo(choiceContext,1, base.Owner);
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(choiceContext);

                //총알 사용
                IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
                foreach (IBulletPowerInterface item in enumerable)
                {
                    item.UseBullet(choiceContext, this, play.Target,base.Owner.Creature, 1);
                }
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
