using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Core;

public class ReloadAction : GameAction
{
    public Player Player { get; }

    public override ulong OwnerId => Player.NetId;

    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    public ReloadAction(Player player)
    {
        Player = player;
    }

    protected override async Task ExecuteAction()
    {
        StS2HoshinoMain.Logger.Info($"[ReloadAction] Executing reload for player {Player.NetId}");

        PlayerChoiceContext context = new GameActionPlayerChoiceContext(this);

        await ReloadCmd.Execute(context, Player);

        StS2HoshinoMain.Logger.Info($"[ReloadAction] Reload complete for player {Player.NetId}");
    }

    public override INetAction ToNetAction()
    {
        return new NetReloadAction();
    }

    public override string ToString()
    {
        return $"ReloadAction owner={Player.NetId}";
    }
}
