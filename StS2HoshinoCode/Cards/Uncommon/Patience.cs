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

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

public class Patience() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Arrival)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    private decimal _extraEnergyFromPlays;

    private decimal ExtraDamageFromPlays
    {
        get
        {
            return _extraEnergyFromPlays;
        }
        set
        {
            AssertMutable();
            _extraEnergyFromPlays = value;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new EnergyVar("ExtraCost", 1)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
    }
    
    public async Task OnInvade(PlayerChoiceContext choiceContext, CardModel card)
    {
        base.DynamicVars.Energy.BaseValue += base.DynamicVars["ExtraCost"].BaseValue;
        ExtraDamageFromPlays +=  base.DynamicVars["ExtraCost"].BaseValue;
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars.Damage.BaseValue += ExtraDamageFromPlays;
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
