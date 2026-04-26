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
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class SuppressionAttack() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override int AmmoCost => 1;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    private bool hasExhausted = false;
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        hasExhausted = false;
        IReadOnlyList<Creature> enemies = base.CombatState!.HittableEnemies;
        
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState!)
            .WithHitFx("vfx/vfx_heavy_blunt", sfx: "shotgunfireheavy.mp3".SfxPath())
            .Execute(choiceContext);
        
        //총알 사용
        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable)
        {
            item.UseBulletForMulti(choiceContext, this, enemies, base.Owner.Creature, 1);
        }
        CardPile pile = PileType.Hand.GetPile(base.Owner);
        CardModel? cardModel = base.Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        if (cardModel != null)
        {
            await CardCmd.Exhaust(choiceContext, cardModel);
            hasExhausted = true;
        }
        await Cmd.Wait(0.25f);
    }

    protected override PileType GetResultPileType()
    {
        if (hasExhausted)
        {
            PileType resultPileType = base.GetResultPileType();
            if (resultPileType != PileType.Discard)
            {
                return resultPileType;
            }
        }
        return PileType.Hand;
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
