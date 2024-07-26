using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using BenchmarkDotNet.Validators;
using Microsoft.CodeAnalysis;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;
using MulTyPlayerClient.GUI.Views;
using Riptide;
using Colors = System.Windows.Media.Colors;
using HSD_Pick = MulTyPlayerClient.GUI.Models.HSD_Pick;

namespace MulTyPlayerClient;

public class HSD_DraftsHandler
{
    public static ObservableCollection<HSD_PlayerViewModel> Team1 { get; } = new();
    public static ObservableCollection<HSD_PlayerViewModel> Team2 { get; } = new();
    public static ObservableCollection<HSD_PlayerViewModel> Team1Backgrounds { get; } = new();
    public static ObservableCollection<HSD_PlayerViewModel> Team2Backgrounds { get; } = new();
    public static ObservableCollection<HSD_PickViewModel> Picks { get; } = new();
    public static int CurrentTeam1PlayerId;
    public static int CurrentTeam2PlayerId;
    public static HSD_Team CurrentTeam;

    public HSD_DraftsHandler()
    {
        ModelController.HSD_Draft.ReadyToStart = false;
        if (Team1Backgrounds.Count != 4)
        {
            Team1Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team1));
            Team1Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team1));
            Team1Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team1));
            Team1Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team1));
        }
        if (Team2Backgrounds.Count != 4)
        {
            Team2Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team2));
            Team2Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team2));
            Team2Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team2));
            Team2Backgrounds.Add(new HSD_PlayerViewModel(new HSD_PlayerModel(null), HSD_Team.Team2));
        }
        Application.Current.Dispatcher.Invoke(() =>
        {
            Team1.Clear(); 
            Team2.Clear();
            Picks.Clear();
        });
    }

    public void RequestJoinTeam(HSD_Team team)
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_PlayerJoined);
        message.AddInt((int)team);
        Client._client.Send(message);
    }

    public void LeaveTeam()
    {
        if (!TryRemovePlayer(Client._client.Id))
            return;
        Message message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_PlayerLeft);
        Client._client.Send(message);
    }

    public void AddPlayer(ushort clientId, HSD_Team team)
    {
        if (!PlayerHandler.TryGetPlayer(clientId, out var player))
            return;
        
        if (clientId == Client._client.Id)
        {
            switch (team)
            {
                case HSD_Team.Team1:
                    ModelController.HSD_Draft.SwapTeam1Enabled = false;
                    ModelController.HSD_Draft.SwapTeam2Enabled = true;
                    break;
                case HSD_Team.Team2:
                    ModelController.HSD_Draft.SwapTeam2Enabled = false;
                    ModelController.HSD_Draft.SwapTeam1Enabled = true;
                    break;
            }
        }

        var playerViewModel = new HSD_PlayerViewModel(new HSD_PlayerModel(player), team);
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (team)
            {
                case HSD_Team.Team1:
                    Team1.Add(playerViewModel);
                    break;
                case HSD_Team.Team2:
                    Team2.Add(playerViewModel);
                    break;
                default:
                    Logger.Write("[HSD] Team does not exist.");
                    break;
            }
        });

        ModelController.HSD_Draft.ReadyToStart = Team1.Count > 0 && Team2.Count > 0;
    }

    public void RemovePlayer(ushort clientId, HSD_Team team)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (team)
            {
                case HSD_Team.Team1:
                    Team1.Remove(Team1.FirstOrDefault(p => p.PlayerModel.Id == clientId));
                    break;
                case HSD_Team.Team2:
                    Team2.Remove(Team2.FirstOrDefault(p => p.PlayerModel.Id == clientId));
                    break;
                default:
                    Logger.Write("[HSD] Team does not exist.");
                    break;
            }
        });
    }

    public bool TryRemovePlayer(ushort clientId)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            HSD_Team? team = null;
            if (Team1.Any(x => x.PlayerModel.Id == clientId))
                team = HSD_Team.Team1;
            if (Team2.Any(x => x.PlayerModel.Id == clientId))
                team = HSD_Team.Team2;
            if (team == null)
                return false;
            RemovePlayer(clientId, (HSD_Team)team);
            return true;
        });
    }
        
    [MessageHandler((ushort)MessageID.HSD_PlayerJoined)]
    public static void HandlePlayerJoined(Message message)
    {
        Client.HDrafts.AddPlayer(message.GetUShort(), (HSD_Team)message.GetInt());
    }
    
    [MessageHandler((ushort)MessageID.HSD_PlayerLeft)]
    public static void HandlePlayerLeft(Message message)
    {
        Client.HDrafts.TryRemovePlayer(message.GetUShort());
    }
    
    [MessageHandler((ushort)MessageID.HSD_TeamData)]
    public static void HandleTeamData(Message message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Team1.Clear(); 
            Team2.Clear();
            Picks.Clear();
        });
        var team1Array = message.GetUShorts();
        var team2Array = message.GetUShorts();
        var pickCount = message.GetInt();
        for (var pIndex = 0; pIndex < pickCount; pIndex++)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Picks.Add(new HSD_PickViewModel(
                    new HSD_PickModel(
                        message.GetInt(), 
                        (HSD_Team)message.GetInt(), 
                        (HSD_Pick)message.GetInt())));
            });
        }
        CurrentTeam = (HSD_Team)message.GetInt();
        CurrentTeam1PlayerId = message.GetUShort();
        CurrentTeam2PlayerId = message.GetUShort();
        foreach(var p in team1Array)
            Client.HDrafts.AddPlayer(p, HSD_Team.Team1);
        foreach(var p in team2Array)
            Client.HDrafts.AddPlayer(p, HSD_Team.Team2);
    }

    [MessageHandler((ushort)MessageID.HSD_PickRequest)]
    public static void HandlePickRequest(Message message)
    {
        var levelId = message.GetInt();
        var team = (HSD_Team)message.GetInt();
        var pick = (HSD_Pick)message.GetInt();
        ModelController.HSD_Draft.SetAvailability(levelId, false);
        Application.Current.Dispatcher.Invoke(() => 
            { Picks.Add(new HSD_PickViewModel(new HSD_PickModel(levelId, team, pick))); });
        CurrentTeam = (HSD_Team)message.GetInt();
        CurrentTeam1PlayerId = message.GetUShort();
        CurrentTeam2PlayerId = message.GetUShort();
        
        Application.Current.Dispatcher.Invoke(HighLightActive);
        
        if ((CurrentTeam1PlayerId == Client._client.Id && CurrentTeam == HSD_Team.Team1 ||
            CurrentTeam2PlayerId == Client._client.Id && CurrentTeam == HSD_Team.Team2) && Picks.Count < 8)
        {
            ModelController.HSD_Draft.AllowLevelSelect = true;
        }
        else
        {
            ModelController.HSD_Draft.AllowLevelSelect = false;
        }
    }

    private static void HighLightActive()
    {
        foreach (var p in HSD_DraftsHandler.Team1) p.TeamColor = App.AppColors.MainAccent;
        foreach (var p in HSD_DraftsHandler.Team2) p.TeamColor = App.AppColors.AltAccent;
        if (Picks.Count == 8)
            return;
        if (CurrentTeam == HSD_Team.Team1)
            Team1.FirstOrDefault(x => x.PlayerModel.Id == CurrentTeam1PlayerId)!.TeamColor = new SolidColorBrush(Colors.White);
        else if (CurrentTeam == HSD_Team.Team2) 
            Team2.FirstOrDefault(x => x.PlayerModel.Id == CurrentTeam2PlayerId)!.TeamColor = new SolidColorBrush(Colors.White);
    }


    [MessageHandler((ushort)MessageID.HSD_Start)]
    public static void HandleStartRequest(Message message)
    {
        ModelController.HSD_Draft.ReadyToStart = false;
        ModelController.HSD_Draft.SwapTeam1Enabled = false;
        ModelController.HSD_Draft.SwapTeam2Enabled = false;
        Application.Current.Dispatcher.Invoke(() => { Picks.Clear(); });
        CurrentTeam = (HSD_Team)message.GetInt();
        CurrentTeam1PlayerId = message.GetUShort();
        CurrentTeam2PlayerId = message.GetUShort();

        Application.Current.Dispatcher.Invoke(HighLightActive);
        
        if (CurrentTeam1PlayerId == Client._client.Id && CurrentTeam == HSD_Team.Team1 ||
            CurrentTeam2PlayerId == Client._client.Id && CurrentTeam == HSD_Team.Team2)
        {
            ModelController.HSD_Draft.AllowLevelSelect = true;
        }
        else
        {
            ModelController.HSD_Draft.AllowLevelSelect = false;
        }
    }

    [MessageHandler((ushort)MessageID.HSD_Reset)]
    public static void HandleReset(Message message)
    {
        var team = HSD_Team.NoTeam;
        if (Team1.Any(x => x.PlayerModel.Id == Client._client.Id))
            team = HSD_Team.Team1;
        else if (Team2.Any(x => x.PlayerModel.Id == Client._client.Id))
            team = HSD_Team.Team2;
        Client.HHideSeek.StartDraftsSession(Picks.ToArray(), team);
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            LobbyViewModel.DraftWindow.Close();
        });
        Client.HDrafts = new HSD_DraftsHandler();
    }
}

public enum HSD_Team
{
    NoTeam,
    Team1,
    Team2
}
