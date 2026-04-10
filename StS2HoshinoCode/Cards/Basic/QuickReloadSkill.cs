using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Keywords;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Basic;

public class QuickReloadSkill() : StS2HoshinoCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Reload)
    ];

    
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ReloadCmd.Execute(choiceContext, base.Owner);
        CardPlayStartedEntry? cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card != this);
        if (cardPlayStartedEntry != null)
        {
            await CardPileCmd.Add(cardPlayStartedEntry.CardPlay.Card, PileType.Draw, CardPilePosition.Top);
        }
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
