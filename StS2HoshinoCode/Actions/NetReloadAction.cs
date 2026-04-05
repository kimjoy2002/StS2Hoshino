using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace StS2Hoshino.StS2HoshinoCode.Core;


public struct NetReloadAction : INetAction, IPacketSerializable
{
    public GameAction ToGameAction(Player player)
    {
        return new ReloadAction(player);
    }

    public void Serialize(PacketWriter writer)
    {
    }

    public void Deserialize(PacketReader reader)
    {
    }

    public override string ToString()
    {
        return "NetReloadAction";
    }
}
