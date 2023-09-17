using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using Riptide.Utils;
using System;
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

            PlayerHandler.Players.Clear();
            
            _client.Connect(_ip, 5, 0, authentication);

            ModelController.KoalaSelect.OnProceedToLobby += () =>
            {
                KoalaSelected = true;
            };
            ModelController.Lobby.OnLogout += () =>
            {
                KoalaSelected = false;
            };

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
            HPlayer = new PlayerHandler();
        }

        private static void InitRiptide()
        {
            RiptideLogger.Initialize(Logger.Instance.Write, true);
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
            HSync.SetMemAddrs();
            HSync.RequestSync();
        }

        private static void Disconnected(object sender, Riptide.DisconnectedEventArgs e)
        {
            ClientThreadToken.Cancel();
            IsConnected = false;
            ModelController.KoalaSelect.MakeAllAvailable();
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.ResetPlayerList));
            SFXPlayer.PlaySound(SFX.PlayerDisconnect);

            if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
            {
                Logger.Instance.Write("Initiating reconnection attempt.");
                InitRiptide();
                Message authentication = Message.Create();
                authentication.AddString(_pass);
                _client.Connect(_ip, 5, 0, authentication);
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    WindowHandler.SettingsWindow.Hide();
                    Logger.Instance.Log.Clear();
                });
        }

        private static void ConnectionFailed()
        {            
            ClientThreadToken.Cancel();
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
        }

        private static void ClientLoop()
        {
            while (!ClientThreadToken.Token.IsCancellationRequested)
            {
                if (IsConnected && TyProcess.FindProcess() && KoalaSelected)
                {
                    try
                    {
                        if (!HGameState.IsAtMainMenuOrLoading())
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
                        Logger.Instance.Write("TyClosedException:\n" + e.ToString());
                    }
                    catch (TyProcessException e)
                    {
                        Logger.Instance.Write("TyProcessException:\n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Write(e.ToString());
                    }
                }
                _client.Update();
                Thread.Sleep(MS_PER_TICK);
            }
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            Logger.Instance.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
        }
    }
}
