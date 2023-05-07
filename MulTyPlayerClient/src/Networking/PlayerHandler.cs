using MulTyPlayerClient.GUI;
using Riptide;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayerClient.Memory;
using MulTyPlayerClient.Networking;
using MulTyPlayerCommon;
using MulTyPlayerClient.GUI.Views;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.Networking
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
            Koala koala = new(koalaName, Array.IndexOf(KoalaReplication.KoalaNames, koalaName));
            Players.Add(clientID, new Player(koala, name, clientID, isHost, false));
            PlayerInfo player = new(clientID, name, koalaName);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () =>
                {
                    MainViewModel.Lobby.PlayerInfoList.Add(player);
                });
            MainViewModel.KoalaSelect.SetAvailability(koalaName, false);
            /*
            if (!WindowHandler.KoalaSelectWindow.IsVisible)
                BasicIoC.SFXPlayer.PlaySound(SFX.PlayerConnect);
            */
        }

        public static void AnnounceSelection(string koalaName, string name, bool isHost)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            message.AddString(koalaName);
            message.AddString(name);
            message.AddUShort(ConnectionService.Client.Id);
            message.AddBool(isHost);
            PlayerInfo player = new(ConnectionService.Client.Id, name, koalaName);
            MainViewModel.Lobby.PlayerInfoList.Add(player);
            ConnectionService.Client.Send(message);
            Replication.KoalaSelected = true;
        }

        [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
        public static void RemovePlayer(Message message)
        {
            ushort id = message.GetUShort();
            Players.Remove(id);
            
            MainViewModel.KoalaSelect.SetAvailability(Players[id].Koala.KoalaName, true);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () =>
                {
                    if (MainViewModel.Lobby.TryGetPlayerInfo(id, out PlayerInfo playerInfo))
                        MainViewModel.Lobby.PlayerInfoList.Remove(playerInfo);
                }
            );
        }

        [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
        public static void PeerDisconnected()
        {
            
            //SFXPlayer.PlaySound(SFX.PlayerDisconnect);
        }

        public static bool TryGetLocalPlayer(out Player player)
        {
            return Players.TryGetValue(ConnectionService.Client.Id, out player);
        }
    }
}
