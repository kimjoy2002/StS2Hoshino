using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

public class SurpriseAttack() : StS2HoshinoCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IInvade
{	
    //private const string _decreaseKey = "Decrease";

    private decimal _extraDamage;

    private decimal ExtraDamage
    {
        get
        {
            return _extraDamage;
        }
        set
        {
            AssertMutable();
            _extraDamage = value;
        }
    }

    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move),
        new DynamicVar("Decrease", 2m)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
            ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(choiceContext);
    }
    
    

    public Task OnInvade(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }
        decimal baseValue = base.DynamicVars["Decrease"].BaseValue;
        base.DynamicVars.Damage.BaseValue -= baseValue;
        ExtraDamage -= baseValue;
        return Task.CompletedTask;
    }
    
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars.Damage.BaseValue -= ExtraDamage;
    }
}
