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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.CardModels;

namespace StS2Hoshino.StS2HoshinoCode.Relics;

public class HoshinoRelic : HoshinoBaseRelic, IReloadRelic
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Unpowered)];
    
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public async Task OnReload(PlayerChoiceContext choiceContext, Player reloader, int amount)
    {
        Flash();
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, null);
    }
    
}