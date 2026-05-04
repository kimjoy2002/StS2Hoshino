using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

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
        new DamageVar(12, ValueProp.Move),
        new PowerVar<WeakPower>(1m)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithAttackerAnim("Swing", 0.15f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
    
    public async Task OnInvade(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        if (card != this)
        {
            return;
        }

        
        List<Creature> validTargets = base.CombatState!.HittableEnemies.Where<Creature>((Func<Creature, bool>) (c => c.IsAlive)).ToList<Creature>();
        if (validTargets.Count > 0)
        {
            Creature? singleTarget = RunState!.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) validTargets);
            if (singleTarget != null)
            {
                int amount = base.DynamicVars["WeakPower"].IntValue;
                await PowerCmd.Apply<WeakPower>(choiceContext, singleTarget, amount, base.Owner.Creature, this);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
