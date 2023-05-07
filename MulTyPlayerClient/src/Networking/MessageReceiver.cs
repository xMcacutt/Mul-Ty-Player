using MulTyPlayerClient.GUI;
using MulTyPlayerClient.Sync;
using Riptide;
using System;
using System.Linq;
using MulTyPlayerCommon;
using System.Windows.Threading;
using System.Windows;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.Networking
{
    internal static class MessageReceiver
    {
        [MessageHandler((ushort)MessageID.Ready)]
        public static void PeerReady(Message message)
        {
            PlayerHandler.Players[message.GetUShort()].IsReady = message.GetBool();
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    new Action(MainViewModel.Lobby.UpdateReadyStatus));
        }

        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            ConnectionService.Client.Disconnect();
        }

        [MessageHandler((ushort)MessageID.ReqHost)]
        public static void RequestHostResponse(Message message)
        {
            if (message.GetBool())
            {
                ChatLog.Write("You have been made host. You now have access to host only commands.");
                PlayerHandler.Players[ConnectionService.Client.Id].IsHost = true;

                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                        new Action(MainViewModel.Lobby.UpdateHostIcon));
                return;
            }
            ChatLog.Write($"Client {PlayerHandler.Players.Values.First(p => p.IsHost)} already has host privileges");
        }

        [MessageHandler((ushort)MessageID.HostChange)]
        public static void HostChange(Message message)
        {
            PlayerHandler.Players[message.GetUShort()].IsHost = true;
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    new Action(MainViewModel.Lobby.UpdateHostIcon));
        }

        [MessageHandler((ushort)MessageID.HostCommand)]
        public static void HostCommandResponse(Message message)
        {
            ChatLog.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.ResetSync)]
        private static void HandleSyncReset(Message message)
        {
            ChatLog.Write("Synchronisations have been reset to new game state.");
            Replication.HSync = new SyncHandler();
        }

        [MessageHandler((ushort)MessageID.P2PMessage)]
        private static void HandleMessageFromPeer(Message message)
        {
            ChatLog.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.Countdown)]
        public static void StartCountdown(Message message)
        {
            Countdown.Start();
        }
    }
}
