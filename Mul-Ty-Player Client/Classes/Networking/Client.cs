using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayer;
using MulTyPlayerClient.Classes.GamePlay;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;
using MulTyPlayerClient.GUI.Views;
using MulTyPlayerClient.Objectives;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using Steamworks;
using DisconnectedEventArgs = Riptide.DisconnectedEventArgs;

namespace MulTyPlayerClient;

internal class Client
{
    public const int MS_PER_TICK = 20;
    public static bool IsConnected;
    public static bool KoalaSelected;
    public static Riptide.Client _client;
    public static string _ip;
    private static string _pass;
    public static SteamId? SteamId;
    public static VIP VIP;
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
    public static ChaosHandler HChaos;
    public static HSD_DraftsHandler HDrafts;

    public static CancellationTokenSource cts;
    public static bool Relaunching => TyProcess.LaunchingGame;

    public static void Start(string ip, string name, string pass)
    {
        try
        {
            InitHandlers();
            var parsedIP = IPHandler.ParseIP(ip);
            if (parsedIP == "")
                throw new InvalidIPException();
            _ip = parsedIP;
            _pass = pass;
            Name = name;
            ModelController.Login.WasConnectionError = true;

            InitRiptide();
            cts = new CancellationTokenSource();

            var authentication = Message.Create();
            authentication.AddByte((byte)ConnectionType.Login);
            authentication.AddString(_pass);
            authentication.AddBool(ModelController.Login.JoinAsSpectator);
            ulong steamId = SteamId ?? 0;
            VIPHandler.VIPs.TryGetValue(steamId, out VIP);
            
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
    
    public static void StartClientCountRequest(string ip)
    {
        var parsedIP = IPHandler.ParseIPSilently(ip);
        if (parsedIP is null)
        {
            ModelController.Login.CurrentServerClientCount = "?";
            return;
        }
        RiptideLogger.Initialize(Console.WriteLine, false);
        var miniClient = new Riptide.Client();
        miniClient.ConnectionFailed += HandleClientCountRequestResponse;
        cts = new CancellationTokenSource();
        var authentication = Message.Create();
        authentication.AddByte((byte)ConnectionType.ClientCountRequest);
        if (!parsedIP.Contains(':'))
            parsedIP += ":" + SettingsHandler.Settings.Port;
        var attempt = miniClient.Connect(parsedIP, 1, 0, authentication);
        if (!attempt)
            ModelController.Login.CurrentServerClientCount = "?";
        Thread loop = new(() =>
        {
            while(!cts.IsCancellationRequested)
                miniClient.Update();
        });  
        loop.Start();
    }

    private static void HandleClientCountRequestResponse(object sender, ConnectionFailedEventArgs eventArgs)
    {
        if (eventArgs.Message is null)
        {
            ModelController.Login.CurrentServerClientCount = "?";
            return;
        }
        var connectionFailedType = (ConnectionFailedType)eventArgs.Message.GetByte();
        var reason = eventArgs.Message.GetString();
        ModelController.Login.CurrentServerClientCount = eventArgs.Message.GetInt().ToString();
        cts.Cancel();
    }

    private static void InitHandlers()
    {
        HGameState = new GameStateHandler();
        HLevel = new LevelHandler();
        HSync = new SyncHandler();
        HHero = new HeroHandler();
        HKoala = new KoalaHandler();
        HCommand = new CommandHandler();
        HPlayer = new PlayerHandler();
        HObjective = new ObjectiveHandler();
        HHideSeek = new HSHandler();
        HGlow = new GlowHandler();
        HChaos = new ChaosHandler();
        HDrafts = new HSD_DraftsHandler();
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
                    PlayerHandler.Players.Add(new Player(null, Client.Name, Client._client.Id, false, false, HSRole.Spectator, 0, VIP));
                    PlayerHandler.AnnounceSelection(null, Client.Name, false, false, HSRole.Spectator);
                    SFXPlayer.PlaySound(SFX.PlayerConnect);
                    KoalaSelected = true;
                });
        }
        if (SettingsHandler.DoLevelLock)
            HSync.HLevelLock.RequestData();
        PerkHandler.DeactivateAllPerks();
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
        Application.Current.Dispatcher.Invoke(() => { LobbyViewModel.DraftWindow?.Close(); });
        Application.Current.Dispatcher.Invoke(() => { PerkHandler.PerkDialog?.Close(); });
        SFXPlayer.PlaySound(SFX.PlayerDisconnect);
        if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
        {
            IsReconnect = true;
            Logger.Write("Initiating reconnection attempt.");
            InitRiptide();
            var authentication = Message.Create();
            authentication.AddByte((byte)ConnectionType.Login);
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
        if (eventArgs.Message != null)
        {
            var connectionFailedType = (ConnectionFailedType)eventArgs.Message.GetByte();
            var reason = eventArgs.Message.GetString();
            Debug.WriteLine($"ERROR: Riptide connection failed. Reason: {reason}");
        }
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
        SpectatorHandler.ToggleFreeCam(false, true);
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
                            if (SettingsHandler.GameMode == GameMode.HideSeek)
                                HHideSeek.Run();
                            else
                            {
                                HSync.CheckEnabledObservers();
                                if (SettingsHandler.DoTESyncing)
                                    HObjective.RunChecks();
                            }
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
                            if (HLevel.CurrentLevelData.Id == Levels.OutbackSafari.Id)
                                HHero.SetHeroState(BullState.Splat);
                            else
                                HHero.SetHeroState(HeroState.CreditsLock);
                            HGameState.SetCameraState(CameraState.FreeCam);
                            HGameState.CheckLoadChanged();
                            HLevel.GetCurrentLevel();
                            if (SettingsHandler.GameMode != GameMode.HideSeek)
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

public enum ConnectionType : byte
{
    Login,
    ClientCountRequest,
}

public enum ConnectionFailedType : byte
{
    IncorrectPassword,
    Timeout,
    WasClientCountRequest,
    ServerFull
}