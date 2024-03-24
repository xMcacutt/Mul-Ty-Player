using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.Views;
using MulTyPlayerClient.Objectives;
using Riptide;
using Riptide.Utils;

namespace MulTyPlayerClient;

internal class Client
{
    public const int MS_PER_TICK = 20;
    public static bool IsConnected;
    public static bool KoalaSelected;
    public static Riptide.Client _client;
    public static string _ip;
    private static string _pass;
    public static string Name;
    public static Koala OldKoala;
    public static bool IsReconnect = false;

    public static KoalaHandler HKoala;
    public static PlayerHandler HPlayer;
    public static GameStateHandler HGameState;
    public static CommandHandler HCommand;
    public static HeroHandler HHero;
    public static LevelHandler HLevel;
    public static SyncHandler HSync;
    public static ObjectiveHandler HObjective;
    public static HSHandler HHideSeek;
    public static GlowHandler HGlow;

    public static CancellationTokenSource cts;
    public static bool Relaunching => TyProcess.LaunchingGame;

    public static void Start(string ip, string name, string pass)
    {
        try
        {
            InitHandlers();
            _ip = ip;
            _pass = pass;
            Name = name;

            InitRiptide();
            cts = new CancellationTokenSource();

            var authentication = Message.Create();
            authentication.AddString(_pass);
            authentication.AddBool(ModelController.Login.JoinAsSpectator);
            if (!_ip.Contains(':'))
                _ip += ":" + SettingsHandler.Settings.Port;
            var attempt = _client.Connect(_ip, 5, 0, authentication);
            if (!attempt) ConnectionAttemptFailed();
            ModelController.KoalaSelect.OnProceedToLobby += () => { KoalaSelected = true; };
            ModelController.Lobby.OnLogout += () => { KoalaSelected = false; };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        Thread loop = new(() => ClientLoop(cts.Token));
        loop.Start();
    }

    private static void InitHandlers()
    {
        //PlayerReplication.
        HLevel = new LevelHandler();
        HSync = new SyncHandler();
        HGameState = new GameStateHandler();
        HHero = new HeroHandler();
        HKoala = new KoalaHandler();
        HCommand = new CommandHandler();
        HPlayer = new PlayerHandler();
        HObjective = new ObjectiveHandler();
        HHideSeek = new HSHandler();
        HGlow = new GlowHandler();
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
        if (IsReconnect)
        {
            if (!ModelController.Login.JoinAsSpectator)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    () =>
                    {
                        ModelController.KoalaSelect.KoalaClicked(OldKoala, false);
                    });
            }
            KoalaSelected = true;
            IsReconnect = false;
        }
        if (ModelController.Login.JoinAsSpectator)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () =>
                {
                    PlayerHandler.Players.Add(new Player(null, Client.Name, Client._client.Id, false, false, HSRole.Spectator));
                    PlayerHandler.AnnounceSelection(null, Client.Name, false);
                    SFXPlayer.PlaySound(SFX.PlayerConnect);
                    KoalaSelected = true;
                });
        }
        if (SettingsHandler.DoLevelLock)
            HSync.HLevelLock.RequestData();
    }

    private static void Disconnected(object sender, DisconnectedEventArgs e)
    {
        cts.Cancel();
        KoalaSelected = false;
        IsConnected = false;
        foreach (var koalaID in PlayerReplication.PlayerTransforms.Keys) 
            PlayerReplication.ReturnKoala(koalaID);
        PlayerReplication.ClearPlayers();
        ModelController.KoalaSelect.MakeAllAvailable();
        SFXPlayer.PlaySound(SFX.PlayerDisconnect);
        if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
        {
            IsReconnect = true;
            Logger.Write("Initiating reconnection attempt.");
            InitRiptide();
            var authentication = Message.Create();
            authentication.AddString(_pass);
            HKoala = new KoalaHandler();
            HPlayer = new PlayerHandler();
            var attempt = _client.Connect(_ip, 5, 0, authentication);
            if (attempt)
            {
                cts = new CancellationTokenSource();
                Thread loop = new(() => ClientLoop(cts.Token));
                loop.Start();
                return;
            }
        }
        IsReconnect = false;
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () =>
            {
                if(App.SettingsWindow is not null) 
                    App.SettingsWindow.Hide(); 
                ModelController.Lobby.Logout();
            });
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
        IsReconnect = false;
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () =>
            {
                if(App.SettingsWindow is not null) 
                    App.SettingsWindow.Hide(); 
                ModelController.Lobby.Logout();
            });
    }

    private static void ConnectionAttemptFailed()
    {
        Debug.WriteLine("ERROR: Riptide connection failed.\nReason:" +
                        "An attempt to connect was not made. Most likely an invalid IP address.");
        cts.Cancel();
        ModelController.Login.ConnectionAttemptSuccessful = false;
        ModelController.Login.ConnectionAttemptCompleted = true;
    }

    private static void ClientLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (TyProcess.FindProcess())
            {
                try
                {
                    if (KoalaSelected && !ModelController.Login.JoinAsSpectator)
                    {
                        HGameState.CheckMainMenuOrLoading();
                        if (!HGameState.IsOnMainMenuOrLoading)
                        {
                            HGameState.CheckLoadChanged();
                            HLevel.GetCurrentLevel();
                            if (!SettingsHandler.DoHideSeek)
                            {
                                HSync.CheckEnabledObservers();
                                if (SettingsHandler.DoTESyncing)
                                    HObjective.RunChecks();
                            }
                            else
                                HHideSeek.Run();

                            HHero.GetTyPosRot();
                            HKoala.CheckTA();
                        }
                        HHero.SendCoordinates();
                        // Writes all received player coordinates into koala positions in memory
                        PlayerReplication.RenderKoalas(MS_PER_TICK);
                    }
                    if (ModelController.Login.JoinAsSpectator)
                    {
                        SpectatorHandler.UpdateCamera();
                        HGameState.CheckMainMenuOrLoading();
                        if (!HGameState.IsOnMainMenuOrLoading)
                        {
                            var lockedHeroState = HLevel.CurrentLevelData.Id == Levels.OutbackSafari.Id ? 5 : 50;
                            HHero.SetHeroState(lockedHeroState);
                            HGameState.SetCameraState(28);
                            HGameState.CheckLoadChanged();
                            HLevel.GetCurrentLevel();
                            if (!SettingsHandler.DoHideSeek)
                            {
                                HSync.CheckEnabledObservers();
                                if (SettingsHandler.DoTESyncing)
                                    HObjective.RunChecks();
                            }
                            HKoala.CheckTA();
                        }
                        PlayerReplication.RenderKoalas(MS_PER_TICK);
                    }
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