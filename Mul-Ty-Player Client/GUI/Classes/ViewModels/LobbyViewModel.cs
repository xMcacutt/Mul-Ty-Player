using System;
using System.Collections.ObjectModel;
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
        Lobby.CanLaunchGameChanged += Model_CanLaunchGameChanged;
        Countdown.OnCountdownBegan += OnCountdownBegan;
        Countdown.OnCountdownAborted += OnCountdownEnded;
        Countdown.OnCountdownFinished += OnCountdownEnded;

        Logger.OnLogWrite += ChatMessages.Add;

        //i dont know what this does but i think matt said it was important. idk why.
        ModelController.KoalaSelect.OnKoalaSelected += k =>
        {
            CollectionViewSource.GetDefaultView(ChatMessages).Refresh();
        };
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

    private static LobbyModel Lobby => ModelController.Lobby;

    public void OnEntered()
    {
        IsReadyButtonEnabled = Client.HGameState.IsAtMainMenu();
        Lobby.UpdateReadyStatus();
        Lobby.UpdateHostIcon();
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

    private void OnCountdownEnded()
    {
        IsReadyButtonEnabled = IsOnMenu;
    }

    private void OnCountdownBegan()
    {
        IsReadyButtonEnabled = false;
    }
}