using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

internal class PlayerHandler
{
    public static ObservableCollection<Player> Players = new();
    
    public PlayerHandler()
    {
        Players = new ObservableCollection<Player>();
    }

    //Adds other player to players & playerInfo
    public static void AddPlayer(Koala koala, string name, ushort clientId, bool isHost, bool isReady, HSRole role)
    {
        if (Players.Any(x => x.Id == clientId))
            Players.Remove(Players.First(x => x.Id == clientId));
        Players.Add(new Player(koala, name, clientId, isHost, isReady, role));
        ModelController.KoalaSelect.SetAvailability(koala, false);
        SFXPlayer.PlaySound(SFX.PlayerConnect);
        PlayerReplication.AddPlayer((int)koala);
    }
    
    //Adds yourself to players
    public static void AnnounceSelection(Koala koala, string name, bool isHost, bool isReady = false, HSRole role = HSRole.Hider)
    {
        var clientId = Client._client.Id;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
        message.AddString(Enum.GetName(typeof(Koala), koala));
        message.AddString(name);
        message.AddUShort(Client._client.Id);
        message.AddBool(isHost);
        message.AddBool(isReady);
        message.AddInt((int)role);
        if (Players.Any(x => x.Id == clientId))
            Players.Remove(Players.First(x => x.Id == clientId));
        Players.Add(new Player(koala, name, clientId, isHost, isReady, role));
        Client._client.Send(message);
        Client.KoalaSelected = true;
    }

    public static void RemovePlayer(ushort clientId)
    {
        ModelController.KoalaSelect.SetAvailability(Players[clientId].Koala, true);
        PlayerReplication.RemovePlayer((int)Players[clientId].Koala);
        if (Players.Any(x => x.Id == clientId))
            Players.Remove(Players.First(x => x.Id == clientId));
    }

    public static bool TryGetPlayer(ushort clientId, out Player player)
    {
        player = Players.FirstOrDefault(x => x.Id == clientId);
        return player != null;
    }

    [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
    public static void PeerDisconnected(Message message)
    {
        var clientId = message.GetUShort();
        RemovePlayer(clientId);
        //VoiceHandler.TryRemoveVoice(clientId);
        SFXPlayer.PlaySound(SFX.PlayerDisconnect);
    }
    
    public static bool HostExists()
    {
        return Players.Any(p => p.IsHost);
    }
    
    public void SetReady()
    {
        var player = Players.First(x => x.Id == Client._client.Id);
        player.IsReady = !player.IsReady;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
        message.AddBool(player.IsReady);
        Client._client.Send(message);
    }
    
    [MessageHandler((ushort)MessageID.Ready)]
    public static void PeerReady(Message message)
    {
        Players[message.GetUShort()].IsReady = message.GetBool();
    }
    
}