using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class PlayerInfo
    {
        public ushort ClientID { get; set; }
        public BitmapImage KoalaIcon { get; set; }
        public string PlayerName { get; set; }
        public bool IsHost { get; set; }
        public bool IsReady { get; set; }

        public PlayerInfo(ushort clientID, string playerName, string koalaName) 
        {
            ClientID = clientID;
            PlayerName = playerName;
            var koalaIconPath = $"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Icons/{koalaName}.png";
            KoalaIcon = new BitmapImage(new Uri(koalaIconPath));
            KoalaIcon.Freeze();
            IsHost = PlayerHandler.Players[clientID].IsHost;
            IsReady = PlayerHandler.Players[clientID].IsReady;
        }
    }
}
