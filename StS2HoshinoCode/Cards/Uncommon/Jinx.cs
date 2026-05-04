using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
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
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class Jinx() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override int AmmoCost { get; set; } = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(5m),
        new ExtraDamageVar(7m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => 
        {
            int s3 = AmmoClass.GetSlot3UsedCount(card.Owner);
            int s4 = AmmoClass.GetSlot4UsedCount(card.Owner);
            if (card.Pile != null && card.Pile.Type == PileType.Play)
            {
                int current = AmmoClass.GetCurrentAmmo(card.Owner);
                int max = AmmoClass.GetMaxAmmo(card.Owner);
                if (current + 1 == max - 2) s3--;
                if (current + 1 == max - 3) s4--;
            }
            return (decimal)(s3 - s4);
        })
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");

        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).FromCard(this).Targeting(play.Target!)
            .WithHitFx(sfx: "shotgunfire.mp3".SfxPath())
            .Execute(choiceContext);

        // 총알 효과 처리
        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable)
        {
            item.UseBullet(choiceContext, this, play.Target!, base.Owner.Creature, 1);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(3m);
    }
}
