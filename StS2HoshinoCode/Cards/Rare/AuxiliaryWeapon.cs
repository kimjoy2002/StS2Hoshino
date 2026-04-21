using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class AuxiliaryWeapon() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardModel cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), context: choiceContext, player: base.Owner, filter: null, source: this)).FirstOrDefault();
        if (cardModel != null)
        {
            await CardCmd.Exhaust(choiceContext, cardModel);
        }

        {
            CardModel card = cardModel.CreateClone();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true,CardPilePosition.Top);
        }
        {
            CardModel card = cardModel.CreateClone();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, addedByPlayer: true);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
