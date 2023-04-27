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

        public static void AddPlayer(string koalaName, string name, ushort clientID, bool isHost)
        {
            Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
            Players.Add(clientID, new Player(koala, name, clientID, isHost, false));
            PlayerInfo player = new(clientID, name, koalaName);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    BasicIoC.MainGUIViewModel.PlayerInfoList.Add(player);
                });
            BasicIoC.KoalaSelectViewModel.SetAvailability(koalaName, false);
            if (!WindowHandler.KoalaSelectWindow.IsVisible)
                BasicIoC.SFXPlayer.PlaySound(SFX.PlayerConnect);
        }

        public static void AnnounceSelection(string koalaName, string name, bool isHost)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            message.AddString(koalaName);
            message.AddString(name);
            message.AddUShort(Client._client.Id);
            message.AddBool(isHost);
            PlayerInfo player = new(Client._client.Id, name, koalaName);
            BasicIoC.MainGUIViewModel.PlayerInfoList.Add(player);
            Client._client.Send(message);
            Client.KoalaSelected = true;
        }

        public static void RemovePlayer(ushort id)
        {
            BasicIoC.KoalaSelectViewModel.SetAvailability(Players[id].Koala.KoalaName, true);
            Players.Remove(id);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    if (BasicIoC.MainGUIViewModel.TryGetPlayerInfo(id, out PlayerInfo playerInfo))
                        BasicIoC.MainGUIViewModel.PlayerInfoList.Remove(playerInfo);
                }
            );            
        }

        [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
        public static void PeerDisconnected(Message message)
        {
            RemovePlayer(message.GetUShort());
            BasicIoC.SFXPlayer.PlaySound(SFX.PlayerDisconnect);
        }

        public static bool TryGetLocalPlayer(out Player player)
        {
            return Players.TryGetValue(Client._client.Id, out player);
        }
    }
}
