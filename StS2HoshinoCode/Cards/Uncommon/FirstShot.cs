using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class FirstShot() : StS2HoshinoCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IInvade
{
    public override int AmmoCost { get; set; } = 1;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Arrival)
    ];
    protected override HashSet<CardTag> CanonicalTags => [StS2HoshinoCard.BulletCard];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(choiceContext);
        
        //총알 사용
        IEnumerable<IBulletPowerInterface> enumerable = base.Owner.Creature.Powers.OfType<IBulletPowerInterface>();
        foreach (IBulletPowerInterface item in enumerable)
        {
            item.UseBullet(choiceContext, this, play.Target,base.Owner.Creature, 1);
        }
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }

    public async Task OnInvade(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        if (card == this)
        {
            CardModel copy_card = CreateClone();
            await CardPileCmd.AddGeneratedCardToCombat(copy_card, PileType.Hand, player);
        }
    }
}
