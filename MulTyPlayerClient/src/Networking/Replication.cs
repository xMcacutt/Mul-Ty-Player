using MulTyPlayerClient.GUI;
using Riptide;
using Riptide.Utils;
using System;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayerClient.Memory;
using MulTyPlayerClient.Settings;
using MulTyPlayerClient.Sync;
using MulTyPlayerClient.GUI.Views;
using MulTyPlayerClient.GUI.ViewModels;
using System.Windows.Data;

namespace MulTyPlayerClient.Networking
{
    internal class Replication
    {
        public static bool Relaunching => TyProcess.LaunchingGame;
        public static bool KoalaSelected = false;
        public static string Name;
        public static Koala OldKoala;

        public static PlayerInfo PlayerInfo;

        public static KoalaReplication KoalaHandler;
        public static PlayerHandler HPlayer;
        public static GameStateHandler HGameState;
        public static CommandHandler HCommand;
        public static PlayerTransform PlayerTransform;
        public static LevelHandler HLevel;
        public static SyncHandler HSync;

        public const int MS_PER_TICK = 20;

        private static LobbyViewModel lobby;
        private static Thread loop;
        private static CancellationTokenSource cts;

        public static void RegisterConnectionService()
        {
            Reset();
            ConnectionService.ConnectionFailed += Cancel;
            ConnectionService.Disconnected += Cancel;
            ConnectionService.ConnectionSuccessful += BeginReplication;
        }

        //Called when connection between client and server drops out, keeps all the data
        //syncing stuff intact without having to resync
        public static void BeginReplication()
        {
            loop = new(ReplicationLoop);
            loop.Start();
        }

        public static void Cancel()
        {
            cts.Cancel();
        }

        //Should only be called at start or sync reset
        private static void Reset()
        {
            HLevel = new LevelHandler();
            HSync = new SyncHandler();
            HGameState = new GameStateHandler();
            LevelHandler.OnLevelChange += PlayerTransform.CheckOutbackSafari;
            KoalaHandler = new KoalaReplication();
            HCommand = new CommandHandler();
        }

        public static void RegisterLobby(LobbyViewModel lobbyViewModel)
        {
            lobby = lobbyViewModel;
        }

        private static void ReplicationLoop()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (ConnectionService.IsConnected && TyProcess.FindProcess())
                {
                    try
                    {
                        if (!HGameState.CheckLoaded())
                        {
                            HLevel.GetCurrentLevel();
                            HSync.CheckEnabledObservers();
                            KoalaHandler.CheckTA();
                            PlayerTransform.Update();
                            MessageSender.SendPlayerCoordinates(PlayerTransform.Position, PlayerTransform.Rotation);
                        }                        
                    }
                    catch (Exception ex) when (ex is TyClosedException || ex is TyProcessException)
                    {
                        ChatLog.Write(ex.Message);
                    }
                }
                ConnectionService.Client.Update();
                Thread.Sleep(MS_PER_TICK);
            }
        }
    }
}
