using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Hook;
using StS2Hoshino.StS2HoshinoCode.Keywords;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Relics;

public class HoshinoRelic : HoshinoBaseRelic, IOnReloaded
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(3, ValueProp.Unpowered),
        new PowerVar<ExpertPower>(3m)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(HoshinoKeywords.Reload),
            HoverTipFactory.FromPower<ExpertPower>()];
    public override RelicRarity Rarity => RelicRarity.Starter;
    

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Flash();
            await PowerCmd.Apply<ExpertPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 3, base.Owner.Creature, null);
        }
    }
    public async Task OnReload(PlayerChoiceContext ctx, Player player, bool useButton)
    {
        if (base.Owner == player)
        {
            Flash();
        }
    }
}