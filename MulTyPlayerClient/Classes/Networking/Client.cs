using Microsoft.VisualBasic.Logging;
using MulTyPlayerClient.GUI;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using Steamworks.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

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

        public static CancellationTokenSource cts;

        public static EventHandler<ConnectionFailedEventArgs> connectionFailedHandler = delegate { ConnectionFailed(); };
        public static EventHandler<ConnectionFailedEventArgs> connectionFailedReconnectHandler = delegate { AutoReconnect.ConnectionFailed(); };

        public const int MS_PER_TICK = 20;

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
            _client.Disconnected += Disconnected;
            _client.ConnectionFailed -= connectionFailedReconnectHandler;
            _client.ConnectionFailed += connectionFailedHandler;

            cts = new CancellationTokenSource();

            Message authentication = Message.Create();
            authentication.AddString(_pass);
            if (!_ip.Contains(':')) { _ip += ":" + SettingsHandler.Settings.Port; }
            _client.Connect(_ip, 5, 0, authentication);

            Thread _loop = new(ClientLoop);
            _loop.Start();
        }

        private static void ClientLoop()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (IsRunning)
                {
                    try
                    {
                        if (ProcessHandler.FindTyProcess())
                        {
                            if (Relaunching)
                            {
                                BasicIoC.LoggerInstance.Write("Ty has been restarted. You're back in!");
                                BasicIoC.SFXPlayer.PlaySound(SFX.MenuAccept);
                                Relaunching = false;
                                continue;
                            }
                            else
                            {
                                if (!HGameState.CheckLoaded())
                                {
                                    HLevel.GetCurrentLevel();
                                    HSync.CheckEnabledObservers();
                                    HHero.GetTyPosRot();
                                    HKoala.CheckTA();
                                }
                                HHero.SendCoordinates();
                            }
                        }
                        else
                        {
                            if (!Relaunching)
                            {
                                throw new TyClosedException();
                            }
                        }
                    }
                    catch (TyClosedException ex)
                    {
                        Relaunching = true;
                        BasicIoC.LoggerInstance.Write(ex.Message);
                        BasicIoC.SFXPlayer.PlaySound(SFX.MenuCancel);
                    }
                }
                _client.Update();
                Thread.Sleep(MS_PER_TICK);
            }         
        }

        private static void Connected()
        {
            BasicIoC.LoginViewModel.SaveDetails();
            BasicIoC.KoalaSelectViewModel.Setup();
            BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = true;
            BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
            IsRunning = true;
        }

        private static void Disconnected(object sender, Riptide.DisconnectedEventArgs e)
        {
            cts.Cancel();
            IsRunning = false;
            BasicIoC.KoalaSelectViewModel.MakeAllAvailable();
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(BasicIoC.MainGUIViewModel.ResetPlayerList));
            BasicIoC.SFXPlayer.PlaySound(SFX.PlayerDisconnect);

            if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
            {
                BasicIoC.LoggerInstance.Write("Initiating reconnection attempt.");
                cts = new CancellationTokenSource();
                _client.ConnectionFailed -= connectionFailedHandler;
                _client.ConnectionFailed += connectionFailedReconnectHandler;
                Message authentication = Message.Create();
                authentication.AddString(_pass);
                if (!_ip.Contains(':')) { _ip += ":" + SettingsHandler.Settings.Port; }
                _client.Connect(_ip, 5, 0, authentication);
                Thread _loop = new(ClientLoop);
                _loop.Start();
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    WindowHandler.KoalaSelectWindow.Hide();
                    WindowHandler.ClientGUIWindow.Hide();
                    WindowHandler.SettingsWindow.Hide();
                    BasicIoC.LoggerInstance.Log.Clear();
                    BasicIoC.LoginViewModel.ConnectEnabled = true;
                    WindowHandler.LoginWindow.Show();
                });
        }

        private static void ConnectionFailed()
        {
            BasicIoC.LoginViewModel.ConnectEnabled = true;
            SystemSounds.Hand.Play();
            MessageBox.Show("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
            cts.Cancel();
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
