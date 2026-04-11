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
using StS2Hoshino.StS2HoshinoCode.Character;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public class RefillAmmo() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int amount = 0;
        IEnumerable<StS2HoshinoCard> enumerable2 = PileType.Exhaust.GetPile(base.Owner).Cards.OfType<StS2HoshinoCard>();
        foreach (StS2HoshinoCard item in enumerable2)
        {
            if (item.Tags.Contains(StS2HoshinoCard.BulletBoxCard))
            {
                await CardPileCmd.Add(item, PileType.Draw);
                amount++;
            }
        }

        if (amount > 0)
        {
            await OnlyDeckShuffle(choiceContext, base.Owner);
            await CardPileCmd.Draw(choiceContext, amount, base.Owner);
        }
    }
    
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
