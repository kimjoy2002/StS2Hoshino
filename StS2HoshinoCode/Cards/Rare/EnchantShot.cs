using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
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
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
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
        IReadOnlyList<Creature> enemies = base.CombatState.HittableEnemies;
        
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_heavy_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        
        //총알 사용
        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable)
        {
            item.UseBulletForMulti(choiceContext, this, enemies, base.Owner.Creature, 1);
        }
    }
    
    public override bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewards, CardCreationOptions options)
    {
        if (player != base.Owner)
        {
            return false;
        }

        bool change_ = false;

        //아마 여기서는 호출이 안될거같은데... patch로 해야하나?
        foreach (CardCreationResult cardReward in cardRewards)
        {
            if (cardReward.Card is EnchantShot)
            {
                List<EnchantmentModel> ableEnchantments = new List<EnchantmentModel>();
                foreach (EnchantmentModel enchantment in ModelDb.DebugEnchantments)
                {
                    if (!(enchantment is DeprecatedEnchantment))
                    {
                        if (enchantment.CanEnchant(cardReward.Card))
                        {
                            ableEnchantments.Add(enchantment);
                        }
                    }
                }
                if (ableEnchantments.Count == 0)
                {
                    break;
                }
                
                CardModel card = base.Owner.RunState.CloneCard(cardReward.Card);
                CardCmd.Enchant<Swift>(card, 1/*수치는 랜덤?*/);
                cardReward.ModifyCard(card);
                change_ = true;

            }
        }
        
        return change_;
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
