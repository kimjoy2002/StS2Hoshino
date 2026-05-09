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
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class ShieldSwing() : StS2HoshinoCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IRunout
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Shield),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Outofammo),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "play.Target");
        bool isRunout = AmmoClass.isEmptyAmmo(Owner);
        
        var attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithAttackerAnim("Swing", 0.15f)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var blockAmount = attackCommand.Results
            .SelectMany(results => results)
            .Sum(r => (r.TotalDamage + r.OverkillDamage) / (isRunout ? 1 : 2));

        await CreatureCmd.GainBlock(
            base.Owner.Creature,
            blockAmount,
            ValueProp.Move,
            cardPlay
        );

        await PowerCmd.Apply<ShieldPower>(
            choiceContext,
            base.Owner.Creature,
            blockAmount,
            base.Owner.Creature,
            this
        );

    }
    
    public Task OnRunout(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
