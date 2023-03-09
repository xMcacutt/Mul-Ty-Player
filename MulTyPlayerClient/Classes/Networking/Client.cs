using MulTyPlayerClient.GUI;
using Riptide;
using Riptide.Utils;
using System;
using System.Linq;
using System.Media;
using System.Net.Cache;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MulTyPlayerClient
{
    internal class Client
    {
        public static bool IsRunning;
        public static bool KoalaSelected = false;
        public static Riptide.Client _client;
        private static Thread _loop;
        private static string _ip;
        private static string _pass;
        public static string Name;

        public static CancellationTokenSource _cts;

        public static KoalaHandler HKoala;
        public static PlayerHandler HPlayer;
        public static GameStateHandler HGameState;
        public static HeroHandler HHero;
        public static LevelHandler HLevel;
        public static SyncHandler HSync;

        public static void StartClient(string ip, string name, string pass)
        {
            SettingsHandler.Setup();
            Logger logger = new(100);
            BasicIoC.LoggerInstance = logger;
            RiptideLogger.Initialize(logger.Write, true);
            _ip = ip;
            _pass = pass;
            Name = name;

            _client = new Riptide.Client();
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected();
            _client.ConnectionFailed += (s, e) => ConnectionFailed();

            Message authentication = Message.Create();
            authentication.AddString(Name);
            authentication.AddString(_pass);
            _client.Connect(_ip + ":8750", 5, 0, authentication);

            _loop = new Thread(new ThreadStart(ClientLoop));
            _loop.Start();
        }

        private static void ClientLoop()
        {
            IsRunning = true;
            while (IsRunning)
            {
                _client.Update();
                Thread.Sleep(10);
            }
        }

        private static void Connected()
        {
            HLevel = new LevelHandler();
            HSync = new SyncHandler();
            HGameState = new GameStateHandler();
            HHero = new HeroHandler();
            HKoala = new KoalaHandler();
            HHero.SetCoordAddrs();
            HKoala.CreateKoalaAddrArrays();
            _cts = new CancellationTokenSource();
            Thread TyDataThread = new(new ParameterizedThreadStart(GameInteractionsLoop));
            TyDataThread.Start(_cts.Token);

            BasicIoC.LoginViewModel.SaveDetails();
            BasicIoC.KoalaSelectViewModel.Setup();
            BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = true;
            BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
        }

        private static void Disconnected()
        {
            IsRunning = false;
        }

        private static void ConnectionFailed()
        {
            BasicIoC.LoginViewModel.ConnectEnabled = true;
            SystemSounds.Hand.Play();
            MessageBox.Show("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
            IsRunning = false;
            BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = false;
            BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
            return;
        }

        public static void GameInteractionsLoop(object token)
        {
            while (!IsRunning) { /*PREVENT MAIN LOOP FROM STARTING UNTIL THE CLIENT IS RUNNING*/ }
            while (IsRunning)
            {
                //GET GAME LOADING STATUS
                HGameState.CheckLoaded();
                if (!HGameState.CheckMenuOrLoading())
                {
                    HLevel.GetCurrentLevel();
                    //NEW LEVEL SETUP STUFF
                    if (!HLevel.bNewLevelSetup)
                    {
                        HKoala.SetCoordAddrs();
                        HLevel.DoLevelSetup();
                        Thread.Sleep(1000); // MAY BE UNECESSARY
                        HLevel.bNewLevelSetup = true;
                    }

                    HHero.SendCoordinates();

                    //OBSERVERS
                    if (SettingsHandler.DoOpalSyncing && HLevel.MainStages.Contains(HLevel.CurrentLevelId)) { SyncHandler.HOpal.CheckObserverChanged(); SyncHandler.HCrate.CheckObserverChanged(); }
                    if (SettingsHandler.DoTESyncing) SyncHandler.HThEg.CheckObserverChanged();
                    if (SettingsHandler.DoCogSyncing) SyncHandler.HCog.CheckObserverChanged();
                    if (SettingsHandler.DoBilbySyncing) SyncHandler.HBilby.CheckObserverChanged();
                    if (SettingsHandler.DoRangSyncing) SyncHandler.HAttribute.CheckObserverChanged();
                    if (SettingsHandler.DoPortalSyncing) SyncHandler.HPortal.CheckObserverChanged();
                    if (SettingsHandler.DoCliffsSyncing) SyncHandler.HCliffs.CheckObserverChanged();
                    
                    HHero.GetTyPosRot();
                    HKoala.SetCoordAddrs();
                    HKoala.CheckTA();
                }
                Thread.Sleep(10);
            }
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            BasicIoC.LoggerInstance.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
            Disconnected();
        }
    }
}
