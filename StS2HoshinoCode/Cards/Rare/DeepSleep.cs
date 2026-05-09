using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class DeepSleep() : StS2HoshinoCard(-1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            var ownerPlayerCombatState = base.Owner.PlayerCombatState;
            return ownerPlayerCombatState != null &&
                   ownerPlayerCombatState.Energy >= base.DynamicVars.Energy.IntValue;
        }
    }


    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(7, ValueProp.Move),
        new EnergyVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int x = ResolveEnergyXValue();
        if (x > 0)
        {
            List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
                orderby c.Rarity, c.Id
                select c).ToList();
            List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, 
                new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 
                0, x))).ToList();
            foreach (CardModel item in list)
            {
                await CardCmd.Exhaust(choiceContext, item);
            }
            
            
            for (int i = 0; i < x; i++)
            {
                await CommonActions.CardBlock(this, play);
            }
            
            if (x >= 3)
            {
                await PowerCmd.Apply<RetainHandPower>(base.Owner.Creature, 
                    1, base.Owner.Creature, this);
            }
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
