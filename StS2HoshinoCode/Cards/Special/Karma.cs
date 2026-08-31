using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Keywords;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Special;

[Pool(typeof(CurseCardPool))]
public sealed class Karma() : StS2HoshinoCard(-1, CardType.Curse, CardRarity.Curse, TargetType.None), IInvade
{
    private const int RequiredCopies = 3;

    public override bool CanBeGeneratedByModifiers => false;
    public override bool CanBeGeneratedInCombat => false;
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable,
        CardKeyword.Ethereal
    ];

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Arrival)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DynamicVar("RequiredCopies", RequiredCopies)
    ];

    public async Task OnInvade(PlayerChoiceContext choiceContext, Player player, CardModel card)
    {
        if (card == this)
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
    }

    public static async Task AddToDeckAndCombine(Player player, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            await CardPileCmd.AddCurseToDeck<Karma>(player);
            await CombineSets(player);
        }
    }

    private static async Task CombineSets(Player player)
    {
        var deck = PileType.Deck.GetPile(player);
        while (true)
        {
            List<CardModel> karmas = deck.Cards.OfType<Karma>().Take(RequiredCopies).Cast<CardModel>().ToList();
            if (karmas.Count < RequiredCopies)
                return;

            CardModel[] cursePool = ModelDb.CardPool<CurseCardPool>()
                .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                .Where(card => card.CanBeGeneratedByModifiers)
                .ToArray();
            if (cursePool.Length == 0)
                return;

            CardModel replacement = player.RunState.Rng.CombatCardSelection.NextItem(cursePool)!;
            await CardPileCmd.RemoveFromDeck(karmas, showPreview: false);
            await CardPileCmd.AddCursesToDeck([replacement], player);
        }
    }
}
