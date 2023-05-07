using PropertyChanged;
using System;
using System.Windows.Media.Imaging;
using MulTyPlayerClient.Networking;
using MulTyPlayerClient.Memory;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class PlayerInfo
    {
        public ushort ClientID
        {
            get; set;
        }
        public BitmapImage KoalaIcon
        {
            get; set;
        }
        public string PlayerName
        {
            get; set;
        }
        public bool IsHost
        {
            get; set;
        }
        public bool IsReady
        {
            get; set;
        }
        public string Level
        {
            get; set;
        }

        public PlayerInfo(ushort clientID, string playerName, string koalaName)
        {
            ClientID = clientID;
            PlayerName = playerName;
            var koalaIconPath = $"pack://application:,,,/Resources/KoalaSelectionAssets/Icons/{koalaName}.png";
            KoalaIcon = new BitmapImage(new Uri(koalaIconPath));
            KoalaIcon.Freeze();
            IsHost = PlayerHandler.Players[clientID].IsHost;
            IsReady = PlayerHandler.Players[clientID].IsReady;
            if (ConnectionService.Client.Id == ClientID)
            {
                LevelHandler.OnLevelChange += (levelId) => Level = Levels.GetLevelData(levelId).Code;
            }
        }
    }
}
