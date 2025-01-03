﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using MulTyPlayerClient;
using System.Windows.Input;
using MulTyPlayer;

namespace MulTyPlayerClient.GUI.Models;

public class LobbyModel
{
    public LobbyModel()
    {
    }
    
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
    
    public void UpdateHost()
    {
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var player))
        {
            Logger.Write("[ERROR] Could not find self in player list.");
            return;
        }
        IsHost = player.IsHost;
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
    
    #region IsHost

    public event Action<bool> IsHostChanged;

    public bool IsHost
    {
        get => isHost;
        set
        {
            isHost = value;
            IsHostChanged(isHost);
        }
    }

    private bool isHost;

    #endregion
    
    
    #region GameMode
    
    public event Action<GameMode> GameModeChanged;

    public GameMode GameMode
    {
        get => gameMode;
        set
        {
            gameMode = value;
            GameModeChanged(gameMode);
        }
    }

    private GameMode gameMode;

    #endregion
    
    #region LevelLock

    public event Action<bool> IsLevelLockEnabledChanged;

    public bool IsLevelLockEnabled
    {
        get => isLevelLockEnabled;
        set
        {
            isLevelLockEnabled = value;
            IsLevelLockEnabledChanged(isLevelLockEnabled);
        }
    }

    private bool isLevelLockEnabled;

    #endregion
    
    #region IsTimer

    public event Action<bool> IsTimerVisibleChanged;

    public bool IsTimerVisible
    {
        get => isTimerVisible;
        set
        {
            isTimerVisible = value;
            IsTimerVisibleChanged(isTimerVisible);
        }
    }

    private bool isTimerVisible;

    #endregion
    
    #region IsVoiceConnected

    public event Action<bool> IsVoiceConnectedChanged;

    public bool IsVoiceConnected
    {
        get => isVoiceConnected;
        set
        {
            isVoiceConnected = value;
            IsVoiceConnectedChanged(isVoiceConnected);
        }
    }

    private bool isVoiceConnected;

    #endregion


    #region IsAbilityCooldown

    public event Action<bool> IsAbilityCooldownVisibleChanged;

    public bool IsAbilityCooldownVisible
    {
        get => isAbilityCooldownVisible;
        set
        {
            isAbilityCooldownVisible = value;
            IsAbilityCooldownVisibleChanged(isAbilityCooldownVisible);
        }
    }

    private bool isAbilityCooldownVisible;

    #endregion

    #region IsRBEnabled

    public event Action<bool> IsReadyButtonEnabledChanged;

    public bool IsReadyButtonEnabled
    {
        get => isReadyButtonEnabled;
        set
        {
            isReadyButtonEnabled = value;
            IsReadyButtonEnabledChanged(isReadyButtonEnabled);
        }
    }

    private bool isReadyButtonEnabled;

    #endregion
    
}