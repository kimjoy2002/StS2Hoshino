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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class DoorBreaching() : StS2HoshinoCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IInvade
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Arrival),
        HoverTipFactory.FromPower<WeakPower>()
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(14, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
        new DynamicVar("PlusWeak", 1m)
    ];
    private decimal _extraWeakFromPlays;

    private decimal ExtraWeakFromPlays
    {
        get
        {
            return _extraWeakFromPlays;
        }
        set
        {
            AssertMutable();
            _extraWeakFromPlays = value;
        }
    }

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        int amount = base.DynamicVars["WeakPower"].IntValue;
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, amount, base.Owner.Creature, this);
    }
    
    public Task OnInvade(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }

        base.DynamicVars["WeakPower"].BaseValue += base.DynamicVars["PlusWeak"].IntValue;
        ExtraWeakFromPlays += base.DynamicVars["PlusWeak"].BaseValue;
        return Task.CompletedTask;
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars["WeakPower"].BaseValue += ExtraWeakFromPlays;
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["PlusWeak"].UpgradeValueBy(1m);
    }
}
