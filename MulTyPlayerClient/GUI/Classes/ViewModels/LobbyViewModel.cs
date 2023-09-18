using MulTyPlayerClient.Classes;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class LobbyViewModel : IViewModel
    {
        public ObservableCollection<PlayerInfo> PlayerInfoList
        {
            get
            {
                return Lobby.PlayerInfoList;
            }
            set{
                Lobby.PlayerInfoList = value; }
        }

        public ObservableCollection<string> ChatMessages {get; set;}
        private bool copyLogMessagesToChat = true;


        public ICommand ManageInputCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        public bool IsOnMenu {get; set;}

        public string Input { get; set; } = "";

        public string ReadyText { get; set; } = "Ready";
        public bool IsReady { get; set; } = true;
        public bool IsReadyButtonEnabled { get; set; } = true;

        public string LaunchGameText { get; set; } = "Launch Game";
        public bool IsLaunchGameButtonEnabled {get; set;}

        private static LobbyModel Lobby => ModelController.Lobby;

        public LobbyViewModel()
        {
            ChatMessages = new ObservableCollection<string>();
            SetCopyLogMessagesToChat(copyLogMessagesToChat);
            ManageInputCommand = new RelayCommand(ManageInput);
            LogoutCommand = new RelayCommand(Logout);

            Lobby.IsOnMenuChanged += Model_IsOnMenuChanged;
            Lobby.IsReadyChanged += Model_IsReadyChanged;
            Lobby.CanLaunchGameChanged += Model_CanLaunchGameChanged;
            Countdown.OnCountdownBegan += OnCountdownBegan;
            Countdown.OnCountdownAborted += OnCountdownEnded;
            Countdown.OnCountdownFinished += OnCountdownEnded;
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

        public void OnEntered()
        {
            Lobby.UpdateReadyStatus();
            Lobby.UpdateHostIcon();
            Input = "";
        }

        public void OnExited()
        {
            Input = "";
        }

        public void SetCopyLogMessagesToChat(bool value)
        {
            copyLogMessagesToChat = value;
            if (copyLogMessagesToChat)
                Logger.OnLogWrite += ChatMessages.Add;
            else
                Logger.OnLogWrite -= ChatMessages.Add;
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

        private void OnCountdownEnded()
        {
            IsReadyButtonEnabled = IsOnMenu;
        }

        private void OnCountdownBegan()
        {
            IsReadyButtonEnabled = false;
        }
    }
}

