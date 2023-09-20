using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using Riptide.Utils;
using System;
using System.Diagnostics;
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

        public static CancellationTokenSource cts;

        public const int MS_PER_TICK = 20;

        public static void Start(string ip, string name, string pass)
        {
            InitHandlers();

            _ip = ip;
            _pass = pass;
            Name = name;

            InitRiptide();

            cts = new CancellationTokenSource();

            Message authentication = Message.Create();
            authentication.AddString(_pass);
            if (!_ip.Contains(':'))
                _ip += ":" + SettingsHandler.Settings.Port;

            bool attempt = _client.Connect(_ip, 5, 0, authentication);
            if (!attempt)
            {
                ConnectionAttemptFailed();
            }
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
            PlayerHandler.Players.Clear();
        }

        private static void InitRiptide()
        {
            RiptideLogger.Initialize(Logger.Write, true);
            _client = new Riptide.Client();
            _client.Connected += OnRiptideConnected;
            _client.Disconnected += Disconnected;
            _client.ConnectionFailed += ConnectionFailed;
        }

        private static void OnRiptideConnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Riptide connected successfully.\n" + e.ToString());
            ModelController.Login.ConnectionAttemptSuccessful = true;
            ModelController.Login.ConnectionAttemptCompleted = true;
            IsConnected = true;
            HLevel.DoLevelSetup();
        }

        private static void Disconnected(object sender, Riptide.DisconnectedEventArgs e)
        {
            cts.Cancel();
            IsConnected = false;
            ModelController.KoalaSelect.MakeAllAvailable();
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.ResetPlayerList));
            SFXPlayer.PlaySound(SFX.PlayerDisconnect);

            if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
            {
                Logger.Write("Initiating reconnection attempt.");
                InitRiptide();
                Message authentication = Message.Create();
                authentication.AddString(_pass);
                _client.Connect(_ip, 5, 0, authentication);
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    WindowHandler.SettingsWindow.Hide();
                });
        }

        private static void ConnectionFailed(object sender, ConnectionFailedEventArgs eventArgs)
        {
            string reason;
            if (eventArgs != null && eventArgs.Message != null)
            {
                reason = eventArgs.Message.GetString();
            }
            else
            {
                reason = "Unknown";
            }
            Debug.WriteLine($"ERROR: Riptide connection failed. Reason: {reason}");             
            cts.Cancel();
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
        }

        private static void ConnectionAttemptFailed()
        {
            Debug.WriteLine("ERROR: Riptide connection failed.\nReason:" + "An attempt to connect was not made. Most likely an invalid IP address.");
            cts.Cancel();
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
        }

        private static void ClientLoop()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (TyProcess.FindProcess() && KoalaSelected)
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
                        PlayerReplication.RenderKoalas();
                    }
                    catch (TyClosedException e)
                    {
                        Logger.Write("TyClosedException:\n" + e.ToString());
                    }
                    catch (TyProcessException e)
                    {
                        Logger.Write("TyProcessException:\n" + e.ToString());
                    }
                    catch (Exception e)
                    {
                        Logger.Write(e.ToString());
                    }
                }
                _client.Update();
            }
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            Logger.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
        }
    }
}
