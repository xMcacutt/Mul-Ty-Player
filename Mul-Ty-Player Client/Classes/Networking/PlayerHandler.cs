using System;
using System.Collections.Generic;
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
    public static Dictionary<ushort, Player> Players = new();

    public PlayerHandler()
    {
        Players = new Dictionary<ushort, Player>();
    }

    //Adds other player to players & playerInfo
    public static void AddPlayer(Koala koala, string name, ushort clientId, bool isHost, bool isReady, HSRole role)
    {
        var koalaName = Koalas.GetInfo[koala].Name;
        Players.Remove(clientId);
        Players.Add(clientId, new Player(koala, name, clientId, isHost, isReady, role));
        PlayerInfo player = new(clientId, name, koalaName, role);
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () => { ModelController.Lobby.PlayerInfoList.Add(player); });
        ModelController.KoalaSelect.SetAvailability(koala, false);
        SFXPlayer.PlaySound(SFX.PlayerConnect);
        PlayerReplication.AddPlayer((int)koala);
    }
    
    //Adds yourself to playerInfo
    public static void AnnounceSelection(string koalaName, string name, bool isHost, bool isReady = false)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
        message.AddString(koalaName);
        message.AddString(name);
        message.AddUShort(Client._client.Id);
        message.AddBool(isHost);
        message.AddBool(isReady);
        PlayerInfo player = new(Client._client.Id, name, koalaName, HSRole.Hider);
        ModelController.Lobby.PlayerInfoList.Add(player);
        Client._client.Send(message);
        Client.KoalaSelected = true;
    }

    public static void RemovePlayer(ushort clientId)
    {
        ModelController.KoalaSelect.SetAvailability(Players[clientId].Koala, true);
        PlayerReplication.RemovePlayer((int)Players[clientId].Koala);
        Players.Remove(clientId);
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () =>
            {
                if (ModelController.Lobby.TryGetPlayerInfo(clientId, out var playerInfo))
                    ModelController.Lobby.PlayerInfoList.Remove(playerInfo);
            }
        );
    }

    [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
    public static void PeerDisconnected(Message message)
    {
        var clientId = message.GetUShort();
        RemovePlayer(clientId);
        VoiceHandler.TryRemoveVoice(clientId);
        SFXPlayer.PlaySound(SFX.PlayerDisconnect);
    }

    public static bool TryGetLocalPlayer(out Player player)
    {
        return Players.TryGetValue(Client._client.Id, out player);
    }
    
    public static bool HostExists()
    {
        return PlayerHandler.Players.Values.Any(p => p.IsHost);
    }
    
    public void SetReady()
    {
        Players[Client._client.Id].IsReady = !Players[Client._client.Id].IsReady;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
        message.AddBool(Players[Client._client.Id].IsReady);
        Client._client.Send(message);
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.UpdateReadyStatus));
    }
    
    [MessageHandler((ushort)MessageID.Ready)]
    public static void PeerReady(Message message)
    {
        Players[message.GetUShort()].IsReady = message.GetBool();
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.UpdateReadyStatus));
    }
    
}