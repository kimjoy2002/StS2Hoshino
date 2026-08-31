using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;

[Pool(typeof(StS2HoshinoCardPool))]
public sealed class WrongTutor() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override string PortraitPath => "wrongtutor.png".CardImagePath();
    public override string CustomPortraitPath => "wrongtutor_p.png".CardImagePath();

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null)
            return;

        foreach (var player in CombatState.Players.Where(player => player.Creature.IsAlive))
        {
            var discardPile = PileType.Discard.GetPile(player);
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
            CardModel? selected = (await CardSelectCmd.FromCombatPile(
                choiceContext,
                discardPile,
                player,
                prefs)).FirstOrDefault();

            if (selected?.Pile?.Type is PileType.Discard or PileType.Draw)
                await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
