using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using Riptide;
using System.Linq;
using System.Media;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Numerics;

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
            PlayerInfo player = new(clientID, name, koalaName);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    BasicIoC.MainGUIViewModel.PlayerInfoList.Add(player);
                }));
            if (BasicIoC.KoalaSelectViewModel.KoalaAvailable(koalaName))
            {
                BasicIoC.KoalaSelectViewModel.SwitchAvailability(koalaName);
            }
            if (!WindowHandler.KoalaSelectWindow.IsVisible) SFXPlayer.PlaySound(SFX.PlayerConnect);
        }

        public static void AnnounceSelection(string koalaName, string name)
        {

            Message message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            message.AddString(koalaName);
            message.AddString(name);
            message.AddUShort(Client._client.Id);
            PlayerInfo player = new(Client._client.Id, name, koalaName);
            BasicIoC.MainGUIViewModel.PlayerInfoList.Add(player);
            Client._client.Send(message);
            Client.KoalaSelected = true;
        }

        public static void RemovePlayer(ushort id)
        {
            if (!BasicIoC.KoalaSelectViewModel.KoalaAvailable(Players[id].Koala.KoalaName))
            {
                BasicIoC.KoalaSelectViewModel.SwitchAvailability(Players[id].Koala.KoalaName);
            }
            Players.Remove(id);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    BasicIoC.MainGUIViewModel.PlayerInfoList.Remove(
                        BasicIoC.MainGUIViewModel.PlayerInfoList.FirstOrDefault(playerInfo => playerInfo.ClientID == id));
                })
            );
            
        }

        [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
        public static void PeerDisconnected(Message message)
        {
            RemovePlayer(message.GetUShort());
            SFXPlayer.PlaySound(SFX.PlayerDisconnect);
        }
    }
}
