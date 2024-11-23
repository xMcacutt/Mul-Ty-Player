using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MulTyPlayer;
using MulTyPlayerClient.Classes;
using MulTyPlayerClient.GUI.Classes.Views;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.Views;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class LobbyViewModel : IViewModel
{
    public LobbyViewModel()
    {
        ChatMessages = new ObservableCollection<string>();
        ManageInputCommand = new RelayCommand(ManageInput);
        LogoutCommand = new RelayCommand(Logout);
        DraftWindow = new HSD_MainWindow();
        Lobby.IsVoiceConnectedChanged += Model_VoiceConnectedChanged;
        Lobby.IsOnMenuChanged += Model_IsOnMenuChanged;
        Lobby.IsReadyChanged += Model_IsReadyChanged;
        Lobby.IsHostChanged += Model_IsHostChanged;
        Lobby.IsTimerVisibleChanged += Model_IsTimerVisibleChanged;
        Lobby.IsAbilityCooldownVisibleChanged += Model_IsAbilityCooldownVisibleChanged;
        Lobby.IsReadyButtonEnabledChanged += Model_IsReadyButtonEnabledChanged;
        Lobby.GameModeChanged += Model_GameModeChanged;
        Lobby.IsLevelLockEnabledChanged += Model_IsLevelLockEnabledChanged;
        Lobby.CanLaunchGameChanged += Model_CanLaunchGameChanged;
        ModelController.Login.JoinAsSpectatorChanged += Model_IsSpectatorChanged;
        HSHandler.OnRoleChanged += Model_RoleChanged;
        HSHandler.OnTimeChanged += Model_TimeChanged;
        MtpCommandAbility.OnAbilityCooldownTimerChanged += Model_AbilityCooldownChanged;
        ChaosHandler.OnShuffleOnStartChanged += Model_ShuffleOnStartChanged;
        Countdown.OnCountdownBegan += OnCountdownBegan;
        Countdown.OnCountdownAborted += OnCountdownEnded;
        Countdown.OnCountdownFinished += OnCountdownEnded;

        Logger.OnLogWrite += ChatMessages.Add;
    }

    public ObservableCollection<string> ChatMessages { get; set; }

    public ICommand ManageInputCommand { get; set; }
    public ICommand LogoutCommand { get; set; }

    public bool IsOnMenu { get; set; }
    public string Input { get; set; } = "";
    public bool IsReady { get; set; } = true;
    public bool IsReadyButtonEnabled { get; set; } = true;
    public bool IsLaunchGameButtonEnabled { get; set; }
    public bool IsVoiceMuted { get; set; }
    public bool IsVoiceConnected { get; set; }
    public bool IsHostMenuButtonEnabled { get; set; } = false;
    public bool IsTimerVisible { get; set; } = false;
    public bool IsAbilityCooldownVisible { get; set; } = false;
    public object IsHideSeekButtonEnabled { get; set; }
    public object IsChaosButtonEnabled { get; set; }
    public object IsCollectionButtonEnabled { get; set; }
    public object IsHardcoreButtonEnabled { get; set; }
    public object IsNoModeButtonEnabled { get; set; }
    public object IsLevelLockEnabled { get; set; }
    public bool IsSpectator { get; set; }
    public string Time { get; set; } = "00:00:00";
    public string AbilityCooldown { get; set; } = "0";
    public HSRole Role { get; set; } = HSRole.Hider;
    public bool ShuffleOnStart { get; set; }

    private static LobbyModel Lobby => ModelController.Lobby;

    public void OnEntered()
    {
        Lobby.UpdateHost();
        Lobby.IsReadyButtonEnabled = true;
        Time = "00:00:00";
        AbilityCooldown = "0";
        Role = Client.HHideSeek.Role;
        Input = "";
    }

    public void OnExited()
    {
        ChatMessages.Clear();
        Input = "";
    }

    public void ManageInput()
    {
        LobbyModel.ProcessInput(Input);
        Input = "";
    }

    private void Logout()
    {
        Client._client.Disconnect();
        Lobby.Logout();
    }

    private void Model_VoiceConnectedChanged(bool value)
    {
        IsVoiceConnected = value;
    }
    
    private void Model_IsOnMenuChanged(bool value)
    {
        IsOnMenu = value;
    }

    private void Model_IsReadyChanged(bool value)
    {
        IsReady = value;
    }
    
    private void Model_RoleChanged(HSRole newRole)
    {
        Role = newRole;
    }
    
    private void Model_IsSpectatorChanged(bool value)
    {
        IsSpectator = value;
    }
    
    private void Model_ShuffleOnStartChanged(bool shuffleOnStart)
    {
        ShuffleOnStart = shuffleOnStart;
    }

    private void Model_CanLaunchGameChanged(bool value)
    {
        IsLaunchGameButtonEnabled = value;
    }
    
    private void Model_IsHostChanged(bool value)
    {
        IsHostMenuButtonEnabled = value;
    }

    private void Model_IsReadyButtonEnabledChanged(bool value)
    {
        if (ModelController.Login.JoinAsSpectator)
        {
            IsReadyButtonEnabled = false;
            return;
        }
        if (Client.HHideSeek.Mode != HSMode.Neutral)
        {
            IsReadyButtonEnabled = false;
            return;
        }
        IsReadyButtonEnabled = value;
    }

    private void Model_GameModeChanged(GameMode value)
    {
        switch (value)
        {
            case GameMode.HideSeek:
                Model_HideSeekSelected();
                break;
            case GameMode.Normal:
                Model_NoModeSelected();
                break;
            case GameMode.Chaos:
                Model_ChaosSelected();
                break;
            case GameMode.Collection:
                Model_CollectionSelected();
                break;
            case GameMode.Hardcore:
                Model_HardcoreSelected();
                break;
        }
    }

    private void Model_HideSeekSelected()
    {
        IsChaosButtonEnabled = false;
        IsCollectionButtonEnabled = false;
        IsNoModeButtonEnabled = false;
        IsHideSeekButtonEnabled = true;
        IsHardcoreButtonEnabled = false;
    }
    
    private void Model_ChaosSelected()
    {
        IsHideSeekButtonEnabled = false;
        IsCollectionButtonEnabled = false;
        IsNoModeButtonEnabled = false;
        IsChaosButtonEnabled = true;
        IsHardcoreButtonEnabled = false;
    }
    
    private void Model_CollectionSelected()
    {
        IsHideSeekButtonEnabled = false;
        IsChaosButtonEnabled = false;
        IsNoModeButtonEnabled = false;
        IsCollectionButtonEnabled = true;
        IsHardcoreButtonEnabled = false;
    }
    
    private void Model_HardcoreSelected()
    {
        IsHideSeekButtonEnabled = false;
        IsChaosButtonEnabled = false;
        IsNoModeButtonEnabled = false;
        IsCollectionButtonEnabled = false;
        IsHardcoreButtonEnabled = true;
    }
    
    private void Model_NoModeSelected()
    {
        IsHideSeekButtonEnabled = false;
        IsChaosButtonEnabled = false;
        IsCollectionButtonEnabled = false;
        IsNoModeButtonEnabled = true;
        IsHardcoreButtonEnabled = false;
    }
    
    private void Model_IsLevelLockEnabledChanged(bool value)
    {
        IsLevelLockEnabled = value;
    }

    private void Model_TimeChanged(int newTime)
    {
        var timeSpan = TimeSpan.FromSeconds(newTime);
        Time = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    private void Model_AbilityCooldownChanged(int value)
    {
        AbilityCooldown = value.ToString();
    }
    
    private void Model_IsTimerVisibleChanged(bool value)
    {
        IsTimerVisible = value;
    }

    private void Model_IsAbilityCooldownVisibleChanged(bool value)
    {
        IsAbilityCooldownVisible = value;
    }

    private void OnCountdownEnded()
    {
        Lobby.IsReadyButtonEnabled = IsOnMenu;
    }

    private void OnCountdownBegan()
    {
        Lobby.IsReadyButtonEnabled = false;
    }

    public static HSD_MainWindow DraftWindow;
    public void OpenDrafts()
    {
        if (DraftWindow is not { IsVisible: true })
        {
            // Create a new instance if it is not open or has been closed
            DraftWindow = new HSD_MainWindow();
            DraftWindow.Show();
        }
        else
        {
            // If the window is already open, bring it to the front
            DraftWindow.Activate();
        }
    }
}