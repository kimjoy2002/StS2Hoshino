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
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

public class TriggerHappy() : StS2HoshinoCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet)
    ];
    private decimal _extraDamageFromTriggerPlays;

    private decimal ExtraDamageFromTriggerPlays
    {
        get
        {
            return _extraDamageFromTriggerPlays;
        }
        set
        {
            AssertMutable();
            _extraDamageFromTriggerPlays = value;
        }
    }
    public override int AmmoCost { get; set; } = 1;
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
        new PowerVar<TriggerHappyPower>(3m)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .Execute(choiceContext);
        await CommonActions.ApplySelf<BlackMarketPower>(this);
        IEnumerable<TriggerHappy> enumerable2 = PileType.Draw.GetPile(base.Owner).Cards.OfType<TriggerHappy>();
        foreach (TriggerHappy item in enumerable2)
        {
            await CardPileCmd.Add(item, PileType.Hand);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["TriggerHappyPower"].UpgradeValueBy(1m);
    }

}
