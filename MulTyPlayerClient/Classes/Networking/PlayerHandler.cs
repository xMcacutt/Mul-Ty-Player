using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using Riptide;
using System.Linq;
using System.Media;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class PlayerHandler
    {
        public static Dictionary<ushort, Player> Players = new();

        public PlayerHandler()
        {
            Players = new();
        }

        public static void AddPlayer(string koalaName, string name, ushort clientID)
        {
            Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
            Players.Add(clientID, new Player(koala, name, clientID));
            if (GUI.BasicIoC.KoalaSelectViewModel.KoalaAvailable(koalaName))
            {
                GUI.BasicIoC.KoalaSelectViewModel.SwitchAvailability(koalaName);
            }
            if (WindowHandler.ClientGUIWindow != null) SFXPlayer.PlaySound(SFX.PlayerConnect);
        }

        public static void AnnounceSelection(string koalaName, string name)
        {

            Message message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            message.AddString(koalaName);
            message.AddString(name);
            message.AddUShort(Client._client.Id);
            Client._client.Send(message);
            Client.KoalaSelected = true;
        }

        public static void RemovePlayer(ushort id)
        {
            if (!GUI.BasicIoC.KoalaSelectViewModel.KoalaAvailable(Players[id].Koala.KoalaName))
            {
                GUI.BasicIoC.KoalaSelectViewModel.SwitchAvailability(Players[id].Koala.KoalaName);
            }
            Players.Remove(id);
        }
    }
}
