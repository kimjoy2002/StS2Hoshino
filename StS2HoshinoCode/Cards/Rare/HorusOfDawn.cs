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
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public class HorusOfDawn() : StS2HoshinoCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Expert),
    ];
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    private int GetCurseCountInDeck()
    {
        var creature = base.Owner.Creature;
        if (creature?.CombatState != null)
        {
            return CardPile.GetCards(base.Owner,
                    PileType.Draw, PileType.Discard, PileType.Exhaust, PileType.Hand)
                .Count(c => c.Rarity == CardRarity.Curse);
        }

        return CardPile.GetCards(base.Owner, PileType.Deck)
            .Count(c => c.Rarity == CardRarity.Curse);
    }

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int curseCount = GetCurseCountInDeck();
        if (curseCount > 0)
        {
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
            await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, curseCount, base.Owner.Creature, null);
            await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, curseCount, base.Owner.Creature, null);
            await PowerCmd.Apply<ExpertPower>(choiceContext, base.Owner.Creature, curseCount, base.Owner.Creature, null);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
