using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Relics;

public class IronHorusRelic : HoshinoBaseRelic
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(9, ValueProp.Unpowered),
        new PowerVar<ShieldPower>(9m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.Static(StaticHoverTip.Block),
            HoverTipFactory.FromKeyword(HoshinoKeywords.Shield)
        ];
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override async Task BeforeCombatStart()
    {
            Flash();
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, null);
            await PowerCmd.Apply<ShieldPower>(base.Owner.Creature, base.DynamicVars["ShieldPower"].IntValue, base.Owner.Creature, null);
    }
}
