using Microsoft.VisualBasic.Logging;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using Riptide;
using Riptide.Utils;
using Steamworks.Data;
using System;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MulTyPlayerClient
{
    internal class Client
    {
        public static bool IsRunning;
        public static bool Relaunching = false;
        public static bool KoalaSelected = false;
        public static Riptide.Client _client;
        private static string _ip;
        private static string _pass;
        public static string Name;
        public static Koala OldKoala;

        public static KoalaHandler HKoala;
        public static PlayerHandler HPlayer;
        public static GameStateHandler HGameState;
        public static CommandHandler HCommand;
        public static HeroHandler HHero;
        public static LevelHandler HLevel;
        public static SyncHandler HSync;

        public static void StartClient(string ip, string name, string pass)
        {
            HLevel = new LevelHandler();
            HSync = new SyncHandler();
            HGameState = new GameStateHandler();
            HHero = new HeroHandler();
            HKoala = new KoalaHandler();
            HCommand = new CommandHandler();
            RiptideLogger.Initialize(BasicIoC.LoggerInstance.Write, true);
            _ip = ip;
            _pass = pass;
            Name = name;

            _client = new Riptide.Client();
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected(s, e);
            _client.ConnectionFailed += (s, e) => ConnectionFailed();

            Message authentication = Message.Create();
            authentication.AddString(Name);
            authentication.AddString(_pass);
            _client.Connect(_ip + ":8750", 5, 0, authentication);

            Thread _loop = new(new ThreadStart(ClientLoop));
            _loop.Start();
        }

        private static void ClientLoop()
        {
            while (!IsRunning) _client.Update();
            while (IsRunning)
            {
                try
                {
                    BasicIoC.LoggerInstance.Write("hello");
                    if (ProcessHandler.MemoryReadDebugLogging || ProcessHandler.MemoryWriteDebugLogging) BasicIoC.LoggerInstance.Write("|----------------> Start of Cycle <----------------|");
                    //GET GAME LOADING STATUS
                    HGameState.CheckLoaded();
                    if (!HGameState.LoadingState)
                    {
                        HLevel.GetCurrentLevel();
                        //NEW LEVEL SETUP STUFF
                        if (!HLevel.bNewLevelSetup)
                        {
                            HKoala.SetCoordAddrs();
                            HLevel.DoLevelSetup();
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
                    _client.Update();
                    Thread.Sleep(10);
                }
                catch (TyClosedException ex)
                {
                    Relaunching = true;
                    BasicIoC.LoggerInstance.Write(ex.Message);
                    SFXPlayer.PlaySound(SFX.MenuCancel);
                    while (ProcessHandler.FindTyProcess() == null)
                    {
                        _client.Update();
                        Thread.Sleep(100);
                    }
                    BasicIoC.LoggerInstance.Write("Ty has been restarted. You're back in!");
                    SFXPlayer.PlaySound(SFX.MenuAccept);
                    Relaunching = false;
                    continue;
                }
            }
            
        }

        private static void Connected()
        {
            HHero.SetCoordAddrs();

            BasicIoC.LoginViewModel.SaveDetails();
            BasicIoC.KoalaSelectViewModel.Setup();
            BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = true;
            BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
            IsRunning = true;
        }

        private static void Disconnected(object sender, DisconnectedEventArgs e)
        {
            if(e.Reason == DisconnectReason.TimedOut)
            {
                if (SettingsHandler.Settings.AttemptReconnect) 
                {
                    _client = new Riptide.Client();
                    _client.Connected += (s, e) => AutoReconnect.Connected();
                    _client.Disconnected += (s, e) => Disconnected(s, e);
                    _client.ConnectionFailed += (s, e) => AutoReconnect.ConnectionFailed();
                    Message authentication = Message.Create();
                    authentication.AddString(Name);
                    authentication.AddString(_pass);

                    while (!IsRunning)
                    {
                        _client.Connect(_ip + ":8750", 1, 0, authentication);
                        Thread.Sleep(200);
                    }
                    Thread _loop = new(new ThreadStart(ClientLoop));
                    _loop.Start();
                }

            }
            IsRunning = false;
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    WindowHandler.KoalaSelectWindow.Close();
                    WindowHandler.ClientGUIWindow.Close();

                }));
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

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            BasicIoC.LoggerInstance.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
        }
    }
}
