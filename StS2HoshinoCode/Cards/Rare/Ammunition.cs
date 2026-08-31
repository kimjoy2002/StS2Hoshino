using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Rare;

[Pool(typeof(StS2HoshinoCardPool))]
public sealed class Ammunition() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override string CustomPortraitPath => "ammunition_depot_p.png".CardImagePath();

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override HashSet<CardTag> CanonicalTags => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Bullet),
        HoverTipFactory.FromKeyword(HoshinoKeywords.Expert)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new DynamicVar("AmmoSlots", 1m),
        new PowerVar<ExpertPower>(10m)
    ];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null)
            return;

        foreach (var player in CombatState.Players)
        {
            if (!player.Creature.IsAlive)
                continue;

            bool hadAmmoSlots = AmmoClass.IsActive(player);
            if (!hadAmmoSlots)
            {
                await PowerCmd.Apply<ExpertPower>(
                    choiceContext,
                    player.Creature,
                    DynamicVars["ExpertPower"].IntValue,
                    Owner.Creature,
                    this);
            }

            await ReloadCmd.AddMaxAmmo(choiceContext, player, DynamicVars["AmmoSlots"].IntValue);
            AmmoClass.SetActive(player, true);
        }

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExpertPower"].UpgradeValueBy(4m);
    }
}
