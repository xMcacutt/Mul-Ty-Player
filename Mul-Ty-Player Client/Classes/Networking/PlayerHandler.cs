using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Classes.Views;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;
using Riptide;

namespace MulTyPlayerClient;

internal class PlayerHandler
{
    public static ObservableCollection<Player> Players = new();

    public PlayerHandler()
    {
        Application.Current.Dispatcher.Invoke(() => { Players.Clear(); });
        Players.CollectionChanged += (s, e) => NotifyScoresChanged();
    }

    //Adds other player to players & playerInfo
    public static void AddPlayer(Koala? koala, string name, ushort clientId, bool isHost, bool isReady, HSRole role, int score, VIP vip, bool playSound)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (Players.Any(x => x.Id == clientId))
                Players.Remove(Players.First(x => x.Id == clientId));
            Players.Add(new Player(koala, name, clientId, isHost, isReady, role, score, vip));
        });
        if (koala != null)
        {
            ModelController.KoalaSelect.SetAvailability((Koala)koala, false);
            if (playSound)
            {
                var sound = vip == VIP.None ? SFX.PlayerConnect : VIPHandler.GetSound(vip);
                SFXPlayer.PlaySound(sound);
            }
            PlayerReplication.AddPlayer((int)koala);
        }
    }
    
    public static void AnnounceSelection(Koala? koala, string name, bool isHost, bool isReady = false, HSRole role = HSRole.Hider, int score = 0, VIP vip = VIP.None)
    {
        var clientId = Client._client.Id;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
        if (koala is null)
            message.AddString("SPECTATOR");
        else
            message.AddString(Enum.GetName(typeof(Koala), koala));
        message.AddString(name);
        message.AddUShort(Client._client.Id);
        message.AddBool(isHost);
        message.AddBool(isReady);
        message.AddInt((int)role);
        message.AddInt(score);
        message.AddInt((int)vip);
        
        if (koala is not null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Players.Any(x => x.Id == clientId))
                    Players.Remove(Players.First(x => x.Id == clientId));
                Players.Add(new Player(koala, name, clientId, isHost, isReady, role, score, vip)); 
            });
        }
        Client._client.Send(message);
    }

    public static void RemovePlayer(ushort clientId)
    {
        if (!TryGetPlayer(clientId, out var player))
        {
            Logger.Write("[Error] Could not find player in player list");
            return;
        }

        if (player.Koala is not null)
        {
            ModelController.KoalaSelect.SetAvailability((Koala)player.Koala, true);
            PlayerReplication.RemovePlayer((int)player.Koala);
        }
        if (Players.Any(x => x.Id == clientId))
            Application.Current.Dispatcher.Invoke(() =>
            {
                Players.Remove(Players.First(x => x.Id == clientId));
            });
    }

    public static bool TryGetPlayer(ushort clientId, out Player player)
    {
        player = Players.FirstOrDefault(x => x.Id == clientId);
        return player != null;
    }
    
    public static void NotifyScoresChanged()
    {
        foreach (var player in Players)
        {
            player.Score = player.Score;
        }
    }

    [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
    public static void PeerDisconnected(Message message)
    {
        var clientId = message.GetUShort();
        if (!PlayerHandler.TryGetPlayer(clientId, out var player))
            Logger.Write("[ERROR] Could not find player in player list");
        else if (player.Koala is not null) 
            PlayerReplication.ReturnKoala((int)player.Koala);
        RemovePlayer(clientId);
        SFXPlayer.PlaySound(SFX.PlayerDisconnect);
    }
    
    public void SetReady()
    {
        if (SettingsHandler.GameMode == GameMode.HideSeek &&
            Client.HHideSeek.CurrentPerk != null &&
            Client.HHideSeek.CurrentPerk.DisplayName == "None")
        {
            PerkHandler.PerkDialog = new HSD_PerkWindow();
            PerkHandler.PerkDialog.ShowDialog();
            return;
        }
        
        var player = Players.First(x => x.Id == Client._client.Id);
        if (ModelController.Login.JoinAsSpectator)
            return;
        player.IsReady = !player.IsReady;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
        message.AddBool(player.IsReady);
        Client._client.Send(message);
    }
    
    [MessageHandler((ushort)MessageID.Ready)]
    public static void PeerReady(Message message)
    {
        var clientId = message.GetUShort();
        if (!TryGetPlayer(clientId, out var player))
        {
            Logger.Write("[Error] Could not find player in player list");
            return;
        }
        player.IsReady = message.GetBool();
    }

    public void ForceToMenu(ushort clickedPlayerId)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ForceMainMenu);
        message.AddUShort(clickedPlayerId);
        Client._client.Send(message);
    }
}