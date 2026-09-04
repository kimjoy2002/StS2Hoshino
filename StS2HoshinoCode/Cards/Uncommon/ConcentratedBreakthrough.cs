using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Hook;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class ConcentratedBreakthrough() : StS2HoshinoCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override int AmmoCost => 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
        
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(13, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(base.CombatState, "base.CombatState");
        IReadOnlyList<Creature> enemies = base.CombatState!.HittableEnemies;
        
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCardCompat(this, play).TargetingAllOpponents(base.CombatState!)
            .WithHoshinoHitFx("vfx/vfx_starry_impact", sfx: "shotgunfireheavy.mp3".SfxPath())
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
        
        //총알 사용
        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable)
        {
            await item.UseBulletForMulti(choiceContext,this, enemies, base.Owner.Creature, 1);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card != this)
        {
            modifiedCost = originalCost;
            return false;
        }

        int emptyAmmoSlots = AmmoClass.GetMaxAmmo(Owner) - AmmoClass.GetCurrentAmmo(Owner);
        modifiedCost = originalCost - emptyAmmoSlots;
        return emptyAmmoSlots > 0;
    }
}
