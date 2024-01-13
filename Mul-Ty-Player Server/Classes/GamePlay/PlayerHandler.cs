using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using MulTyPlayerServer.Classes.Networking.Commands;
using MulTyPlayerServer.Sync.Objective;
using Riptide;

namespace MulTyPlayerServer;

internal class PlayerHandler
{
    public static Dictionary<ushort, Player> Players = new();

    public PlayerHandler()
    {
        Players = new Dictionary<ushort, Player>();
    }

    public static void AddPlayer(string koalaName, string name, ushort clientID, bool isHost)
    {
        Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
        Players.Add(clientID, new Player(koala, name, clientID, isHost, false, true));
    }

    public static void RemovePlayer(ushort id)
    {
        Players.Remove(id);
    }

    public static void AnnounceDisconnect(ushort id)
    {
        var message = Message.Create(MessageSendMode.Reliable, (ushort)MessageID.AnnounceDisconnect);
        message.AddUShort(id);
        Server._Server.SendToAll(message);
    }

    [MessageHandler((ushort)MessageID.PlayerInfo)]
    private static void HandleGettingCoordinates(ushort fromClientId, Message message)
    {
        if (Players.TryGetValue(fromClientId, out var player))
        {
            player.OnMenu = message.GetBool();
            player.CurrentLevel = message.GetInt();
            player.Coordinates = message.GetFloats();
        }
    }
    
    [MessageHandler((ushort)MessageID.Ready)]
    public static void ClientReady(ushort fromClientId, Message message)
    {
        var ready = message.GetBool();
        //string readyStatus = ready? "ready" : "no longer ready";
        Players[fromClientId].IsReady = ready;

        var status = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
        status.AddUShort(fromClientId);
        status.AddBool(ready);
        Server._Server.SendToAll(status, fromClientId);

        //Server.SendMessageToClients($"Client {fromClientId} is {readyStatus}, {PlayerHandler.Players.Count(x => x.Value.IsReady)} / {Server._Server.ClientCount}", true);
        if (Players.Count(x => x.Value.IsReady) == Server._Server.ClientCount)
        {
            foreach (var entry in Players) entry.Value.IsReady = false;
            Program.HSync = new SyncHandler();
            Program.HObjective = new ObjectiveHandler();
            PeerMessageHandler.SendMessageToClients("All clients are ready, starting countdown", true);
            var countdownStart = Message.Create(MessageSendMode.Reliable, MessageID.Countdown);
            Server._Server.SendToAll(countdownStart);
        }
    }
}