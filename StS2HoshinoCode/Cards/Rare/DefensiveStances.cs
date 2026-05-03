using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
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
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class DefensiveStances() : StS2HoshinoCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DefensiveStancesPower>(1m),
    ];

    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.ApplySelf<DefensiveStancesPower>(choiceContext, this);
        // await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        // IEnumerable<Creature> enumerable = base.CombatState.PlayerCreatures.Where((Creature c) => c?.IsAlive ?? false).ToList();
        // foreach (Creature item in enumerable)
        // {
        //     await CommonActions.Apply<ExpertPower>(choiceContext, item.Player, , base.DynamicVars.Summon.BaseValue, this);
        //     await OstyCmd.Summon(choiceContext, item.Player, base.DynamicVars.Summon.BaseValue, this);
        // }
        // // Apply to all players
        // await CommonActions.Apply<DefensiveStancesPower>(choiceContext, base.CombatState.Players, this, 1);
    }
    
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
