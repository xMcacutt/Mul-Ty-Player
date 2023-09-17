using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MulTyPlayerClient.GUI.Models
{
    public class LobbyModel
    {
        public ObservableCollection<PlayerInfo> PlayerInfoList {get; set; }

        #region IsOnMenu
        public event Action<bool> IsOnMenuChanged;
        public bool IsOnMenu
        {
            get
            {
                return isOnMenu;
            }
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
            get
            {
                return canLaunchGame;
            }
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
            get
            {
                return isReady;
            }
            set
            {
                isReady = value;
                IsReadyChanged(isReady);
            }
        }
        private bool isReady;
        #endregion

        public event Action OnLogout;
        public event Action OnCountdownBegin;
        public event Action OnCountdownEnded;

        public LobbyModel()
        {
            PlayerInfoList = new ObservableCollection<PlayerInfo>();
            OnLogout += PlayerInfoList.Clear;
        }

        public static void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            if (input.StartsWith('/'))
            {
                Client.HCommand.ParseCommand(input);
            }
            else
                Logger.Instance.Write(input);
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
            {
                if (PlayerHandler.Players.TryGetValue(player.ClientID, out Player value))
                    player.IsReady = value.IsReady;
            }
        }

        public void UpdateHostIcon()
        {
            foreach (var player in PlayerInfoList)
            {
                player.IsHost = PlayerHandler.Players[player.ClientID].IsHost;
            }
        }

        public void Logout()
        {
            OnLogout?.Invoke();
        }

        public void BeginCountdown()
        {
            OnCountdownBegin?.Invoke();
        }
    }
}
