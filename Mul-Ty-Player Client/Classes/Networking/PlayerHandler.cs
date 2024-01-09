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
    public static void AddPlayer(Koala koala, string name, ushort clientID, bool isHost, bool isReady)
    {
        var koalaName = Koalas.GetInfo[koala].Name;
        Players.Remove(clientID);
        Players.Add(clientID, new Player(koala, name, clientID, isHost, isReady));
        PlayerInfo player = new(clientID, name, koalaName);
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
        PlayerInfo player = new(Client._client.Id, name, koalaName);
        ModelController.Lobby.PlayerInfoList.Add(player);
        Client._client.Send(message);
        Client.KoalaSelected = true;
    }

    // Linq error spam come from client loop
    // client loop calls checkloaded which calls checkmainmenu
    // which tries to set the local playerinfo level to "M/L"
    // local player info is not added until we select a koala
    // so the linq tries to find player where player.id = client.id and fails which throws an exception
    // every frame

    public static void RemovePlayer(ushort id)
    {
        ModelController.KoalaSelect.SetAvailability(Players[id].Koala, true);
        PlayerReplication.RemovePlayer((int)Players[id].Koala);
        Players.Remove(id);
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () =>
            {
                if (ModelController.Lobby.TryGetPlayerInfo(id, out var playerInfo))
                    ModelController.Lobby.PlayerInfoList.Remove(playerInfo);
            }
        );
    }

    [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
    public static void PeerDisconnected(Message message)
    {
        RemovePlayer(message.GetUShort());
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