using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public sealed class WrongTutor() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    public override string PortraitPath => "wrongtutor.png".CardImagePath();
    public override string CustomPortraitPath => "wrongtutor_p.png".CardImagePath();

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target?.Player is not { } targetPlayer || targetPlayer == Owner)
            return;

        if (LocalContext.NetId is not { } localPlayerId)
            return;

        Player caster = Owner;
        var deferredChoiceContext = new HookPlayerChoiceContext(
            targetPlayer,
            localPlayerId,
            GameActionType.CombatPlayPhaseOnly);
        Task choiceTask = ResolveTargetPlayerChoice(deferredChoiceContext, targetPlayer, caster);
        await deferredChoiceContext.AssignTaskAndWaitForPauseOrCompletion(choiceTask);
    }

    private async Task ResolveTargetPlayerChoice(
        PlayerChoiceContext choiceContext,
        Player targetPlayer,
        Player caster)
    {
        var discardPile = PileType.Discard.GetPile(targetPlayer);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        CardModel? selected = (await CardSelectCmd.FromCombatPile(
            choiceContext,
            discardPile,
            targetPlayer,
            prefs)).FirstOrDefault();

        if (selected?.Pile?.Type is not (PileType.Discard or PileType.Draw))
            return;

        CardModel copy = selected.CreateCloneForPlayerCompat(caster);
        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, caster);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
