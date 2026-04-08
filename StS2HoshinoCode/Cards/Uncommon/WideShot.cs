using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

public class WideShot() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override int AmmoCost => 1;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
        
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(base.CombatState, "base.CombatState");
        IReadOnlyList<Creature> enemies = base.CombatState.HittableEnemies;
        
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
        
        //총알 사용
        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable)
        {
            item.UseBulletForMulti(this, enemies, base.Owner.Creature, 1);
        }
        
        CardPile pile = PileType.Hand.GetPile(base.Owner);
        CardModel? cardModel = base.Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        if (cardModel != null)
        {
            await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
