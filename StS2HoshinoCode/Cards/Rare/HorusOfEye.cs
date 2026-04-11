using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class HorusOfEye() : StS2HoshinoCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private bool _grantedBonus = false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<HorusOfEyePower>(1m)
    ];

    protected override HashSet<CardTag> CanonicalTags => [];
    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Top);
    }
    
    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        await base.AfterCardEnteredCombat(card);
        if (card == this)
        {
            _grantedBonus = false;
            await CheckTopCard();
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        await base.AfterCardChangedPiles(card, oldPileType, source);
        await CheckTopCard();
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        await base.AfterCardDrawn(choiceContext, card, fromHandDraw);
        await CheckTopCard();
    }

    private async Task CheckTopCard()
    {
        if (Owner?.Creature?.CombatState == null) return;
        
        var drawPile = PileType.Draw.GetPile(base.Owner);
        bool topDeckBonus = false;
        
        if (drawPile?.Cards.FirstOrDefault() == this)
        {
            topDeckBonus = true;
        }

        if (_grantedBonus != topDeckBonus)
        {
            _grantedBonus = topDeckBonus;
            if (_grantedBonus)
            {
                await CommonActions.ApplySelf<HorusOfEyePower>(this);
            }
            else
            {
                await PowerCmd.Remove<HorusOfEyePower>(base.Owner.Creature);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
