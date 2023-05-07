using PropertyChanged;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MulTyPlayerClient.Networking;
using Riptide.Utils;
using Steamworks.Data;
using MulTyPlayerCommon;
using Riptide;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class LobbyViewModel : BaseViewModel
    {
        public ObservableCollection<PlayerInfo> PlayerInfoList
        {
            get; set;
        }

        public ChatLog ChatLog { get; set; } = new ChatLog(200);

        public ICommand ManageInputCommand
        {
            get; set;
        }

        public string Input
        {
            get; set;
        }

        public bool ReadyEnabled
        {
            get; set;
        }

        public bool IsOnMenu
        {
            get; set;
        }

        public bool IsReadyButtonEnabled => IsOnMenu && ReadyEnabled;


        public bool IsLaunchGameButtonEnabled
        {
            get; set;
        }

        public LobbyViewModel() : base()
        {
            PlayerInfoList = new ObservableCollection<PlayerInfo>();
            ManageInputCommand = new RelayCommand(ManageInput);
            ReadyEnabled = true;
            IsOnMenu = false;
            RiptideLogger.Initialize(ChatLog.Write, true);
        }

        public void ResetPlayerList()
        {
            PlayerInfoList.Clear();
        }

        public void ManageInput()
        {
            if (string.IsNullOrWhiteSpace(Input)) return;
            if (Input.StartsWith('/'))
            {
                Replication.HCommand.ParseCommand(Input);
            }
            else ChatLog.Write(Input);
            Input = null;
        }

        public void UpdateHostIcon()
        {
            foreach (var player in PlayerInfoList)
            {
                player.IsHost = PlayerHandler.Players[player.ClientID].IsHost;
            }
        }

        public void UpdateReadyStatus()
        {
            foreach (var player in PlayerInfoList)
            {
                if (PlayerHandler.Players.TryGetValue(player.ClientID, out Player value))
                    player.IsReady = value.IsReady;
            }
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

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public void ConsoleSend(Message message)
        {
            ChatLog.Write(message.GetString());
        }

        [Riptide.MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private void HandleGettingCoordinates(Riptide.Message message)
        {
            bool onMenu = message.GetBool();
            ushort clientID = message.GetUShort();
            int level = message.GetInt();

            //Set the incoming players current level code
            if (TryGetPlayerInfo(clientID, out PlayerInfo playerInfo))
            {
                if (onMenu)
                {
                    playerInfo.Level = "M/L";
                    return;
                }
                else
                {
                    playerInfo.Level = Levels.GetLevelData(level).Code;
                }
            }
        }
    }
}

