using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using MulTyPlayerServer.Classes.Networking.Commands;
using MulTyPlayerServer.Sync.Objective;
using Riptide;

namespace MulTyPlayerServer;

internal class PlayerHandler
{
    public static ConcurrentDictionary<ushort, Player> Players = new();

    public PlayerHandler()
    {
        Players = new ConcurrentDictionary<ushort, Player>();
    }

    public static void AddPlayer(string koalaName, string name, ushort clientID, bool isHost, HSRole role, VIP vip)
    {
        Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
        Players.TryAdd(clientID, new Player(koala, name, clientID, isHost, false, true, role, vip)); // check if this was succesful, could be causing issues
    }

    public static void RemovePlayer(ushort id)
    {
        Players.Remove(id, out _);
    }

    public static void KillPlayer(ushort id)
    {
        var message = Message.Create(MessageSendMode.Reliable, (ushort)MessageID.Kill);
        Server._Server.Send(message, id);
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
        Players[fromClientId].IsReady = ready;

        var status = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
        status.AddUShort(fromClientId);
        status.AddBool(ready);
        Server._Server.SendToAll(status, fromClientId);

        if (Players.Values.Where(x => x.Role != HSRole.Spectator).All(x => x.IsReady))
        {
            foreach (var entry in Players) 
                entry.Value.IsReady = false;
            if (SettingsHandler.GameMode == GameMode.HideSeek)
                StartHideTimer(SettingsHandler.HideSeekTime);
            else if (SettingsHandler.GameMode == GameMode.Collection)
            {
                CollectionModeHandler.RunCollectionMode();
            }
            else
            {
                PeerMessageHandler.SendMessageToClients("All clients are ready, starting countdown", true);
                StartCountdown();
            }
        }
    }

    private static void StartHideTimer(int hideTimeLength)
    {
        if (HSHandler.Mode != HSMode.Neutral)
            return;
        var hideTimerMessage = Message.Create(MessageSendMode.Reliable, MessageID.HS_HideTimerStart);
        hideTimerMessage.AddInt(hideTimeLength);
        Server._Server.SendToAll(hideTimerMessage);
        HSHandler.StartHideTimer(hideTimeLength);
    }

    private static void StartCountdown()
    {
        if (Program.HChaos.ShuffleOnStart && SettingsHandler.GameMode == GameMode.Chaos)
            ChaosHandler.ForceShuffle();
        Program.HSync = new SyncHandler();
        Program.HObjective = new ObjectiveHandler();
        var countdownStart = Message.Create(MessageSendMode.Reliable, MessageID.Countdown);
        countdownStart.AddString("start");
        Server._Server.SendToAll(countdownStart);
    }

    [MessageHandler((ushort)MessageID.ForceMainMenu)]
    private static void HandleForceMenu(ushort fromClientId, Message message)
    {
        var playerToForce = message.GetUShort();
        var response = Message.Create(MessageSendMode.Reliable, MessageID.ForceMainMenu);
        Server._Server.Send(response, playerToForce);
    }
}