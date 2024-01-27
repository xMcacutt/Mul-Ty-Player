using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MulTyPlayerClient.Classes;
using MulTyPlayerClient.GUI.Models;
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

        Lobby.IsOnMenuChanged += Model_IsOnMenuChanged;
        Lobby.IsReadyChanged += Model_IsReadyChanged;
        Lobby.IsHostChanged += Model_IsHostChanged;
        Lobby.IsTimerVisibleChanged += Model_IsTimerVisibleChanged;
        Lobby.IsHideSeekEnabledChanged += Model_IsHideSeekEnabledChanged;
        Lobby.IsLevelLockEnabledChanged += Model_IsLevelLockEnabledChanged;
        Lobby.CanLaunchGameChanged += Model_CanLaunchGameChanged;
        HSHandler.OnRoleChanged += Model_RoleChanged;
        HSHandler.OnTimeChanged += Model_TimeChanged;
        Countdown.OnCountdownBegan += OnCountdownBegan;
        Countdown.OnCountdownAborted += OnCountdownEnded;
        Countdown.OnCountdownFinished += OnCountdownEnded;

        Logger.OnLogWrite += ChatMessages.Add;
    }

    public ObservableCollection<PlayerInfo> PlayerInfoList
    {
        get => Lobby.PlayerInfoList;
        set => Lobby.PlayerInfoList = value;
    }

    public ObservableCollection<string> ChatMessages { get; set; }

    public ICommand ManageInputCommand { get; set; }
    public ICommand LogoutCommand { get; set; }

    public bool IsOnMenu { get; set; }

    public string Input { get; set; } = "";
    public bool IsReady { get; set; } = true;
    public bool IsReadyButtonEnabled { get; set; } = true;
    public bool IsLaunchGameButtonEnabled { get; set; }
    public bool IsSyncButtonEnabled { get; set; } = true;
    public bool IsHostMenuButtonEnabled { get; set; } = false;
    public bool IsTimerVisible { get; set; } = false;
    public object IsHideSeekButtonEnabled { get; set; }
    public object IsLevelLockEnabled { get; set; }
    public string Time { get; set; } = "00:00:00";
    public HSRole Role { get; set; } = HSRole.Hider;

    private static LobbyModel Lobby => ModelController.Lobby;

    public void OnEntered()
    {
        Lobby.UpdateReadyStatus();
        Lobby.UpdateHostIcon();
        Time = "00:00:00";
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

    private void Model_IsOnMenuChanged(bool value)
    {
        IsOnMenu = value;
        if (!SettingsHandler.DoHideSeek)
            IsReadyButtonEnabled = IsOnMenu;
    }

    private void Model_IsReadyChanged(bool value)
    {
        IsReady = value;
    }

    private void Model_CanLaunchGameChanged(bool value)
    {
        IsLaunchGameButtonEnabled = value;
    }
    
    private void Model_IsHostChanged(bool value)
    {
        IsHostMenuButtonEnabled = value;
    }

    private void Model_IsHideSeekEnabledChanged(bool value)
    {
        IsHideSeekButtonEnabled = value;
        if (!value) Lobby.IsTimerVisible = false;
        IsReadyButtonEnabled = value || IsOnMenu;
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
    
    private void Model_IsTimerVisibleChanged(bool value)
    {
        IsTimerVisible = value;
    }
    
    private void Model_RoleChanged(HSRole newRole)
    {
        if (Client._client == null)
            return;
        var playerInfo = PlayerInfoList.FirstOrDefault(x => x.ClientId == Client._client.Id);
        if (playerInfo == null)
            return;
        Role = Role == HSRole.Hider ? HSRole.Seeker : HSRole.Hider;
        playerInfo.Role = newRole;
    }

    private void OnCountdownEnded()
    {
        IsReadyButtonEnabled = IsOnMenu;
    }

    private void OnCountdownBegan()
    {
        IsReadyButtonEnabled = false;
    }
}