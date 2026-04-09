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

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

public class Jackpot() : StS2HoshinoCard(7, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IInvade
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(33, ValueProp.Move),
        new CardsVar(1),
        new EnergyVar("LossCost", 2)
    ];
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        if (IsUpgraded)
        {
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        }
    }

    public Task OnInvade(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }
        base.EnergyCost.UpgradeBy(-base.DynamicVars["LossCost"].IntValue);
        return Task.CompletedTask;
    }
}
