using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class Revenge() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Replay", 2m)];

    
    public static async Task AutoPlayFromDrawPileWithReplay(
        PlayerChoiceContext choiceContext,
        Player player,
        int count)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
            CardModel? card = PileType.Draw.GetPile(player).Cards.FirstOrDefault<CardModel>();
            if (card != null)
            {
                await CardPileCmd.Add(card, PileType.Play);
                if (!card.Owner.Creature.IsDead)
                {
                    for (int i = 0; i < count; i++)
                    {
                        await CardCmd.AutoPlay(choiceContext, (i == 0) ? card : card.CreateDupe(), null);
                    }
                }
            }
        }
    }
    
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await AutoPlayFromDrawPileWithReplay(choiceContext, base.Owner, base.DynamicVars["Replay"].IntValue);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Replay"].UpgradeValueBy(1m);
    }
}
