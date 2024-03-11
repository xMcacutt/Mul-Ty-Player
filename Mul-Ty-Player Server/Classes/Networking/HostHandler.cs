using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class HostHandler
{
    private static bool HostExists()
    {
        return PlayerHandler.Players.Values.Any(p => p.IsHost);
    }

    [MessageHandler((ushort)MessageID.ReqHost)]
    public static void RequestHost(ushort fromClientId, Message message)
    {
        var acceptRequest = false;
        if (!HostExists())
        {
            acceptRequest = true;
            SetNewHost(fromClientId);
        }

        var hRequest = Message.Create(MessageSendMode.Reliable, MessageID.ReqHost);
        hRequest.AddBool(acceptRequest);
        Server._Server.Send(hRequest, fromClientId);
    }

    [MessageHandler((ushort)MessageID.GiftHost)]
    public static void GiftHost(ushort fromClientId, Message message)
    {
        SetNewHost(message.GetUShort());
    }

    public static void SetNewHost(ushort newHost)
    {
        foreach (var key in PlayerHandler.Players.Keys) PlayerHandler.Players[key].IsHost = false;
        PlayerHandler.Players[newHost].IsHost = true;
        var notifyHostChange = Message.Create(MessageSendMode.Reliable, MessageID.HostChange);
        notifyHostChange.AddUShort(newHost);
        Server._Server.SendToAll(notifyHostChange);
        PeerMessageHandler.SendMessageToClients($"{PlayerHandler.Players[newHost].Name} has been made host", true);
    }
}