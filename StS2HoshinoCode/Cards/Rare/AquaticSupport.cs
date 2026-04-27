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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Keywords;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class AquaticSupport() : StS2HoshinoCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(2m),
        new CardsVar(2)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.ApplySelf<StrengthPower>(this);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }
    
    
    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        await base.AfterCardChangedPiles(card, oldPileType, source);
        if (card == this)
        {
            if (card.Pile != null && card.Pile.Type == PileType.Draw && oldPileType != PileType.Draw)
            {
                if (!StS2Hoshino.StS2HoshinoCode.Patchs.ShufflePatch.IsShuffling || oldPileType != PileType.Discard)
                {
                    if (oldPileType == PileType.Play)
                    {
                        Action? onPlayed = null;
                        onPlayed = () =>
                        {
                            this.Played -= onPlayed;
                            this.EnergyCost.SetUntilPlayed(0);
                        };
                        this.Played += onPlayed;
                    }
                    else
                    {
                        base.EnergyCost.SetUntilPlayed(0);
                    }
                }
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
