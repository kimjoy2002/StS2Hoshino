using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Relics;

public class WhaleTubeRelic : HoshinoBaseRelic
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(3, ValueProp.Unpowered)
    ];

    public override RelicRarity Rarity => RelicRarity.Rare;



    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        await base.AfterCardChangedPiles(card, oldPileType, source);
        if (card.Owner == base.Owner)
        {
            if (card.Pile != null && card.Pile.Type == PileType.Draw && oldPileType != PileType.Draw)
            {
                if (!StS2Hoshino.StS2HoshinoCode.Patchs.ShufflePatch.IsShuffling || oldPileType != PileType.Discard)
                {
                    Flash();
                    await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, null);
                }
            }
        }
    }
}
