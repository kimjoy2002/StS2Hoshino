using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Special;

/// <summary>
/// 징크스 - 특별 카드
/// 탄약을 소모합니다. 적에게 피해를 5 줍니다.
/// 이번 전투동안 사용한 3번째 위치의 탄약당 피해량이 7(10) 증가하고
/// 4번째 위치의 탄약당 피해량이 7(10) 감소합니다.
/// 탄약 슬롯 위치: 최대 탄약 기준으로 왼쪽부터 1번째, 2번째, 3번째, 4번째
/// </summary>
[Pool(typeof(TokenCardPool))]
public class Jinx() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override int AmmoCost { get; set; } = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new IncreaseVar(7m)
    ];



    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");

        // JinxPower가 있는지 확인하고 없으면 추가 (추적 시작)
        var jinxPower = base.Owner.Creature.Powers.OfType<JinxPower>().FirstOrDefault();
        if (jinxPower == null)
        {
            jinxPower = new JinxPower();
            await PowerCmd.Apply(choiceContext, base.Owner.Creature, jinxPower, base.Owner.Creature);
        }

        decimal baseDamage = DynamicVars.Damage.BaseValue;
        decimal increase = DynamicVars.Increase.BaseValue;

        // 전투 중 기록된 슬롯 사용 횟수에 따라 피해량 가감
        decimal finalDamage = baseDamage + (jinxPower.Slot3UsedCount * increase) - (jinxPower.Slot4UsedCount * increase);
        if (finalDamage < 0) finalDamage = 0;

        await DamageCmd.Attack(finalDamage).FromCard(this).Targeting(play.Target!)
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
        DynamicVars.Damage.UpgradeValueBy(0m);
        DynamicVars.Increase.UpgradeValueBy(3m);
    }
}
