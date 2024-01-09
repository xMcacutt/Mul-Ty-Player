using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using MulTyPlayerClient;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.Models;

public class LobbyModel
{
    public LobbyModel()
    {
        PlayerInfoList = new ObservableCollection<PlayerInfo>();
        OnLogout += PlayerInfoList.Clear;
    }

    public ObservableCollection<PlayerInfo> PlayerInfoList { get; set; }

    public event Action OnLogout;
    public event Action OnCountdownBegin;
    public event Action OnCountdownEnded;

    public static void ProcessInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;
        for (var i = 0; i < Client.HCommand.UndoCalls.Count; i++)
        {
            var call = Client.HCommand.UndoCalls.Pop();
            Client.HCommand.Calls.Push(call);
        }
        Client.HCommand.ParseCommand(input);
    }
    
    public static string ProcessRecall(bool up)
    {
        var recall = "";
        if (up)
        {
            Client.HCommand.Calls.TryPop(out recall);
            if(!string.IsNullOrEmpty(recall))
                Client.HCommand.UndoCalls.Push(recall);
        }
        else
        {
            Client.HCommand.UndoCalls.TryPop(out recall);
            if(!string.IsNullOrEmpty(recall)) 
                Client.HCommand.Calls.Push(recall);
            Client.HCommand.UndoCalls.TryPeek(out recall);
        }
        return recall;
    }

    public bool TryGetPlayerInfo(ushort clientID, out PlayerInfo playerInfo)
    {
        try
        {
            playerInfo = PlayerInfoList.First(pInfo => pInfo.ClientID == clientID);
            return true;
        }
        catch
        {
            playerInfo = null;
            return false;
        }
    }

    public void ResetPlayerList()
    {
        PlayerInfoList.Clear();
    }

    public void UpdateReadyStatus()
    {
        foreach (var player in PlayerInfoList)
            if (PlayerHandler.Players.TryGetValue(player.ClientID, out var value))
                player.IsReady = value.IsReady;
    }

    public void UpdateHostIcon()
    {
        foreach (var player in PlayerInfoList) player.IsHost = PlayerHandler.Players[player.ClientID].IsHost;
    }

    public void Logout()
    {
        OnLogout?.Invoke();
    }

    public void BeginCountdown()
    {
        OnCountdownBegin?.Invoke();
    }

    #region IsOnMenu

    public event Action<bool> IsOnMenuChanged;

    public bool IsOnMenu
    {
        get => isOnMenu;
        set
        {
            if (isOnMenu != value)
            {
                isOnMenu = value;
                IsOnMenuChanged(isOnMenu);
            }
        }
    }

    private bool isOnMenu;

    #endregion

    #region CanLaunchGame

    public event Action<bool> CanLaunchGameChanged;

    public bool CanLaunchGame
    {
        get => canLaunchGame;
        set
        {
            canLaunchGame = value;
            CanLaunchGameChanged(canLaunchGame);
        }
    }

    private bool canLaunchGame;

    #endregion

    #region IsReady

    public event Action<bool> IsReadyChanged;

    public bool IsReady
    {
        get => isReady;
        set
        {
            isReady = value;
            IsReadyChanged(isReady);
        }
    }

    private bool isReady;

    #endregion
}