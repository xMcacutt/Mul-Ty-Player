using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using Riptide.Utils;

namespace MulTyPlayerClient;

internal class Client
{
    public const int MS_PER_TICK = 20;
    public static bool IsConnected;
    public static bool KoalaSelected;
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
    public static bool Relaunching => TyProcess.LaunchingGame;

    public static void Start(string ip, string name, string pass)
    {
        InitHandlers();

        _ip = ip;
        _pass = pass;
        Name = name;

        InitRiptide();

        cts = new CancellationTokenSource();

        var authentication = Message.Create();
        authentication.AddString(_pass);
        if (!_ip.Contains(':'))
            _ip += ":" + SettingsHandler.Settings.Port;

        var attempt = _client.Connect(_ip, 5, 0, authentication);
        if (!attempt) ConnectionAttemptFailed();
        ModelController.KoalaSelect.OnProceedToLobby += () => { KoalaSelected = true; };
        ModelController.Lobby.OnLogout += () => { KoalaSelected = false; };

        Thread loop = new(ClientLoop);
        loop.Start();
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
        Debug.WriteLine("Riptide connected successfully.\n" + e);
        ModelController.Login.ConnectionAttemptSuccessful = true;
        ModelController.Login.ConnectionAttemptCompleted = true;
        IsConnected = true;
        HSync.HLevelLock.RequestData();
        if (HGameState.IsAtMainMenuOrLoading()) return;
        HLevel.DoLevelSetup();
    }

    private static void Disconnected(object sender, DisconnectedEventArgs e)
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
            var authentication = Message.Create();
            authentication.AddString(_pass);
            _client.Connect(_ip, 5, 0, authentication);
        }

        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () => { if(App.SettingsWindow is not null) App.SettingsWindow.Hide(); });
    }

    private static void ConnectionFailed(object sender, ConnectionFailedEventArgs eventArgs)
    {
        string reason;
        if (eventArgs != null && eventArgs.Message != null)
            reason = eventArgs.Message.GetString();
        else
            reason = "Unknown";
        Debug.WriteLine($"ERROR: Riptide connection failed. Reason: {reason}");
        cts.Cancel();
        ModelController.Login.ConnectionAttemptSuccessful = false;
        ModelController.Login.ConnectionAttemptCompleted = true;
    }

    private static void ConnectionAttemptFailed()
    {
        Debug.WriteLine("ERROR: Riptide connection failed.\nReason:" +
                        "An attempt to connect was not made. Most likely an invalid IP address.");
        cts.Cancel();
        ModelController.Login.ConnectionAttemptSuccessful = false;
        ModelController.Login.ConnectionAttemptCompleted = true;
    }

    private static void ClientLoop()
    {
        while (!cts.Token.IsCancellationRequested)
        {
#if DEBUG
            //DateTime frameCommence = DateTime.Now;
            //Logger.WriteDebug("Tick began.");
            //DateTime readTime = DateTime.Now;
            //double read_dt = 0.0;
#endif
            if (TyProcess.FindProcess() && KoalaSelected)
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
#if DEBUG
                    //readTime = DateTime.Now;
                    //read_dt = (readTime - frameCommence).TotalMilliseconds;
                    //Logger.WriteDebug($"Finished reading. Elapsed time: {read_dt}ms. Beginning koala render.");
#endif
                    // Writes all received player coordinates into koala positions in memory
                    PlayerReplication.RenderKoalas(MS_PER_TICK);
                }
                catch (TyClosedException e)
                {
                    Logger.Write("TyClosedException:\n" + e);
                }
                catch (TyProcessException e)
                {
                    Logger.Write("TyProcessException:\n" + e);
                }
                catch (Exception e)
                {
                    Logger.Write(e.ToString());
                }

            _client.Update();
#if DEBUG
            //DateTime tickFinished = DateTime.Now;
            //double render_dt = (tickFinished - readTime).TotalMilliseconds;
            //double totalTime = render_dt + read_dt;
            //Logger.WriteDebug($"Finished koala render. Elapsed time for koala render: {render_dt}ms");
            //Logger.WriteDebug($"Total elapsed time this tick: {totalTime}ms");
#endif
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