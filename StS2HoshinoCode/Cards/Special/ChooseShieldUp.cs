using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class ChooseShieldUp() : StS2HoshinoCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HoshinoKeywords.Shield)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedShield").WithMultiplier((CardModel card, Creature? _) => card.Owner.Creature.Block)
    ];
    
    protected override Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
        => OnChoose(choiceContext, play);
    public override async Task OnChoose(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<ShieldPower>(base.Owner.Creature, 
            ((CalculatedVar)base.DynamicVars["CalculatedShield"]).Calculate(null), base.Owner.Creature, this);
    }
}
