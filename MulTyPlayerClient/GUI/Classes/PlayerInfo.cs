using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MulTyPlayerClient.GUI
{
    public class PlayerInfo
    {
        public ushort ClientID { get; set; }
        public BitmapImage KoalaIcon { get; set; }
        public string PlayerName { get; set; }
        public bool IsHost { get; set; }

        public PlayerInfo(ushort clientID, string playerName, string koalaName) 
        {
            ClientID = clientID;
            PlayerName = playerName;
            var koalaIconPath = $"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Icons/{koalaName}.png";
            KoalaIcon = new BitmapImage(new Uri(koalaIconPath));
            IsHost = (clientID == CommandHandler.Host);
        }
    }
}
