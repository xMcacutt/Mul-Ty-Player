using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using Riptide.Utils;
using System;
using System.Diagnostics;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MulTyPlayerClient
{
    internal class Client
    {
        public static bool IsConnected;
        public static bool Relaunching => TyProcess.LaunchingGame;
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

        public static CancellationTokenSource ClientThreadToken;

        public static EventHandler<ConnectionFailedEventArgs> connectionFailedHandler = delegate { ConnectionFailed(); };
        public static EventHandler<ConnectionFailedEventArgs> connectionFailedReconnectHandler = delegate { AutoReconnect.ConnectionFailed(); };

        public const int MS_PER_TICK = 20;

        public static void Start(string ip, string name, string pass)
        {
            InitHandlers();

            _ip = ip;
            _pass = pass;
            Name = name;

            InitRiptide();

            ClientThreadToken = new CancellationTokenSource();

            Message authentication = Message.Create();
            authentication.AddString(_pass);
            if (!_ip.Contains(':'))
                _ip += ":" + SettingsHandler.Settings.Port;

            _client.Connect(_ip, 5, 0, authentication);

            Thread _loop = new(ClientLoop);
            _loop.Start();
        }

        private static void InitHandlers()
        {
            HLevel = new LevelHandler();
            HSync = new SyncHandler();
            HGameState = new GameStateHandler();
            HHero = new HeroHandler();
            HKoala = new KoalaHandler();
            HCommand = new CommandHandler();
        }

        private static void InitRiptide()
        {
            RiptideLogger.Initialize(ModelController.LoggerInstance.Write, true);
            _client = new Riptide.Client();
            _client.Connected += (s, e) => OnRiptideConnected();
            _client.Disconnected += Disconnected;
            _client.ConnectionFailed -= connectionFailedReconnectHandler;
            connectionFailedHandler = (s, e) => ConnectionFailed();
            _client.ConnectionFailed += connectionFailedHandler;            
        }

        private static void OnRiptideConnected()
        {            
            ModelController.Login.ConnectionAttemptSuccessful = true;
            ModelController.Login.ConnectionAttemptCompleted = true;
            IsConnected = true;
        }

        private static void Disconnected(object sender, Riptide.DisconnectedEventArgs e)
        {
            ClientThreadToken.Cancel();
            IsConnected = false;
            ModelController.KoalaSelect.MakeAllAvailable();
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.ResetPlayerList));
            ModelController.SFXPlayer.PlaySound(SFX.PlayerDisconnect);

            if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
            {
                ModelController.LoggerInstance.Write("Initiating reconnection attempt.");
                InitRiptide();
                Message authentication = Message.Create();
                authentication.AddString(_pass);
                _client.Connect(_ip, 5, 0, authentication);
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    WindowHandler.SettingsWindow.Hide();
                    ModelController.LoggerInstance.Log.Clear();
                });
        }

        private static void ConnectionFailed()
        {
            ModelController.Login.EnableLoginButton();
            SystemSounds.Hand.Play();
            MessageBox.Show("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
            ClientThreadToken.Cancel();
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
        }

        private static void ClientLoop()
        {
            while (!ClientThreadToken.Token.IsCancellationRequested)
            {
                if (IsConnected && TyProcess.FindProcess())
                {
                    try
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
                    catch (TyClosedException e)
                    {
                        ModelController.LoggerInstance.Write("TyClosedException:\n" + e.ToString());
                    }
                    catch (TyProcessException e)
                    {
                        ModelController.LoggerInstance.Write("TyProcessException:\n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        ModelController.LoggerInstance.Write(e.ToString());
                    }
                }
                _client.Update();
                Thread.Sleep(MS_PER_TICK);
            }
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            ModelController.LoggerInstance.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
        }
    }
}
