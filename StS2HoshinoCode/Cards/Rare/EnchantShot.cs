using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class EnchantShot() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override int AmmoCost => 1;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        IReadOnlyList<Creature> enemies = base.CombatState!.HittableEnemies;
        
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState!)
            .WithHitFx("vfx/vfx_heavy_blunt", sfx: "shotgunfire.mp3".SfxPath())
            .Execute(choiceContext);
        
        //총알 사용
        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable)
        {
            item.UseBulletForMulti(choiceContext, this, enemies, base.Owner.Creature, 1);
        }
    }
    
    public override void AfterCreated()
    {
        base.AfterCreated();
        if (this.Owner != null && this.Enchantment == null)
        {
            var ableEnchantments = new List<EnchantmentModel>();
            foreach (var enchantment in ModelDb.DebugEnchantments)
            {
                if (!(enchantment is DeprecatedEnchantment) && enchantment.CanEnchant(this))
                {
                    ableEnchantments.Add(enchantment);
                }
            }

            if (ableEnchantments.Count > 0)
            {
                var rng = this.Owner.PlayerRng.Rewards;
                var selected = rng.NextItem(ableEnchantments);
                int amount = GetRandomEnchantmentAmount(selected, rng);
                CardCmd.Enchant(selected.ToMutable(), this, amount);
            }
        }
    }

    private int GetRandomEnchantmentAmount(EnchantmentModel enchantment, Rng rng)
    {
        if (!enchantment.ShowAmount)
        {
            return 1;
        }

        if (enchantment is Swift)
        {
            return rng.NextInt(1, 2);
        }

        if (enchantment is Sharp)
        {
            return rng.NextInt(2, 5); // 2~4
        }

        if (enchantment is Nimble)
        {
            return rng.NextInt(2, 6); // 2~5
        }

        if (enchantment is Vigorous)
        {
            return rng.NextInt(4, 9); // 4~8
        }

        return rng.NextInt(2, 6); // 기본 2~5
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
