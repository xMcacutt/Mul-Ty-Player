using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

public class HSHandler
{
    //HIDE & SEEK HANDLER
    private bool _timerRunning;
    public HSMode Mode;
    private bool _camLocked;
    private bool _applyPerks;
    
    private int _time;
    public int Time
    {
        get => _time;
        set
        {
            if (_time == value) return;
            _time = value;
            OnTimeChanged?.Invoke(value);
        }
    }
    public delegate void TimeChangedEventHandler(int newTime);
    public static event TimeChangedEventHandler OnTimeChanged;
    
    private HSRole _role;
    public HSRole Role
    {
        get => _role;
        set
        {
            if (ModelController.Login.JoinAsSpectator)
            {
                _role = HSRole.Spectator;
                return;
            }
            if (_role == value) 
                return;
            _role = value;
            if (value == HSRole.Seeker)
                _timerRunning = false;
            ChangeRole(Client._client.Id, value);
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)350));
            OnRoleChanged?.Invoke(value);
        }
    }

    private bool _linesVisible;
    public bool LinesVisible
    {
        get => _linesVisible;
        set
        {
            _linesVisible = value;
            if (!_linesVisible)
                Client.HGlow.ReturnGlows();
        }
    }

    public HSPerk CurrentDebuff = new NoPerk();

    public HSPerk CurrentPerk = new NoPerk();
    
    public delegate void RoleChangedEventHandler(HSRole newRole);
    public static event RoleChangedEventHandler OnRoleChanged;

    public HSHandler()
    {
        OnRoleChanged += AnnounceRoleChanged;
        Role = ModelController.Login.JoinAsSpectator ? HSRole.Spectator : HSRole.Hider;
        Mode = HSMode.Neutral;
    }

    public static void Activate()
    {
        Client.HGlow.ReturnGlows();
        //Client.HHideSeek.CurrentPerk = PerkHandler.LevelPerks[Client.HLevel.CurrentLevelId];
        Client.HHideSeek.StartTimerLoop();
    }

    public static void Deactivate()
    {
        Client.HGlow.ReturnGlows();
        Client.HHideSeek.CurrentPerk.Deactivate();
        Client.HHideSeek.CurrentPerk = new NoPerk();
    }
    
    public void Run()
    {
        // HIDE TIME HIDER
        if (Mode == HSMode.HideTime && Role == HSRole.Hider)
        {
            CurrentPerk.ApplyHider();
            CurrentDebuff.ApplyHider();
        }
        
        // HIDE TIME SEEKER
        if (Mode == HSMode.HideTime && Role == HSRole.Seeker)
            LockPlayer();
        
        // SEEK TIME HIDER
        if (Mode == HSMode.SeekTime && Role == HSRole.Hider)
        {
            //CheckDeath();
            if (!_timerRunning)
                _timerRunning = true;
            CurrentPerk.ApplyHider();
            CurrentDebuff.ApplyHider();
        }

        // SEEK TIME SEEKER
        if (Mode == HSMode.SeekTime && Role == HSRole.Seeker)
        {
            Client.HHero.SetSwimSpeed(21f);
            Client.HHero.SetRunSpeed(10.15f);
            CurrentPerk.ApplySeeker();
            CurrentDebuff.ApplySeeker();
        }
    }

    private void CheckDeath()
    {
        if (Role == HSRole.Seeker)
            return;
        var state = Client.HHero.GetHeroState();
        var comparison = Client.HLevel.CurrentLevelId == 10 ? 8 : 28;
        if (state == comparison)
            Role = HSRole.Seeker;
    }

    private void LockPlayer()
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)0));
        _camLocked = true;
        var pos = MtpCommandTeleport.LevelStarts[Client.HLevel.CurrentLevelId];
        Client.HHero.WritePosition(pos[0], pos[1], pos[2], false);
        Client.HHero.SetRunSpeed(0);
        Client.HLevel.LevelBloomSettings.State = true;
        Client.HLevel.LevelBloomSettings.Value = 0;
    }

    [MessageHandler((ushort)MessageID.HS_Catch)]
    private static void Catch(Message message)
    {
        SFXPlayer.PlaySound(SFX.Punch);
        if (Client.HHideSeek.Role == HSRole.Seeker)
            Client.HHideSeek.Time += 15; //NEEDS TESTING
        else
        {
            Client.HHero.KillPlayer();
            Client.HHideSeek.Role = HSRole.Seeker;
        }
    }

    private void AnnounceRoleChanged(HSRole newRole)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_RoleChanged);
        message.AddInt((int)Client.HHideSeek.Role);
        Client._client.Send(message);
    }
    
    [MessageHandler((ushort)MessageID.HS_ProxyRunHideSeek)]
    private static void SetHideSeek(Message message)
    {
        Client.HHero.SetSwimSpeed();
        Client.HHero.SetRunSpeed();
        Client.HHero.SetGravity();
        Client.HHero.SetGlideSpeed();
        Client.HLevel.LevelBloomSettings.RevertToOriginal();
    }

    [MessageHandler((ushort)MessageID.HS_RoleChanged)]
    private static void PeerRoleChanged(Message message)
    {
        var clientId = message.GetUShort();
        var role = message.GetInt();
        ChangeRole(clientId, (HSRole)role);
    }

    private static void ChangeRole(ushort clientId, HSRole role)
    {
        if (PlayerHandler.TryGetPlayer(clientId, out var player)) 
            player.Role = (HSRole)role;
    }

    [MessageHandler((ushort)MessageID.HS_HideTimerStart)]
    private static void HideTimerStart(Message message)
    {
        Client.HGlow.ReturnGlows();
        
        Client.HCommand.Commands["tp"].InitExecute(new string[] {"@s"});
        
        Client.HHideSeek.Mode = HSMode.HideTime;
        
        foreach (var entry in PlayerHandler.Players) 
            entry.IsReady = false;
        ModelController.Lobby.IsReady = false;
        ModelController.Lobby.IsReadyButtonEnabled = false;
        
        SFXPlayer.PlaySound(SFX.HS_HideStart);
       
        if (Client.HHideSeek.Role == HSRole.Hider)
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)750));
        
        var hideTime = message.GetInt();
        Logger.Write(Client.HHideSeek.Role == HSRole.Hider
            ? $"[HIDE AND SEEK] You have {hideTime} seconds to hide!"
            : $"[HIDE AND SEEK] {hideTime} seconds until seeking!");
    }
    
    [MessageHandler((ushort)MessageID.HS_Warning)]
    private static void Warning(Message message)
    {
        var secondsRemaining = message.GetInt();
        SFXPlayer.PlaySound(SFX.HS_Warning);
        Logger.Write(Client.HHideSeek.Role == HSRole.Hider
            ? $"[HIDE AND SEEK] You have {secondsRemaining} seconds to hide!"
            : $"[HIDE AND SEEK] {secondsRemaining} seconds until seeking!");
    }
    
    [MessageHandler((ushort)MessageID.HS_StartSeek)]
    private static void StartSeek(Message message)
    {
        Client.HHideSeek.Mode = HSMode.SeekTime;
        
        (Client.HCommand.Commands["taunt"] as MtpCommandTaunt)?.TauntStopwatch.Restart();
        
        Client.HKoala.ScaleKoalas();
        Client.HGlow.ReturnGlows();
        Client.HLevel.LevelBloomSettings.RevertToOriginal();
        
        SFXPlayer.PlaySound(SFX.HS_SeekStart);
        
        if (Client.HHideSeek.Role == HSRole.Hider)
            Client.HHideSeek._timerRunning = true;
        
        if (Client.HHideSeek.Role == HSRole.Seeker)
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)750));
        
        Logger.Write(Client.HHideSeek.Role == HSRole.Hider
            ? $"[HIDE AND SEEK] The seekers are coming!"
            : $"[HIDE AND SEEK] Go find them!");
    }
    
    [MessageHandler((ushort)MessageID.HS_EndSeek)]
    private static void EndSeek(Message message)
    {
        Client.HHideSeek.Mode = HSMode.Neutral; 
        Client.HHideSeek._timerRunning = false;
        Client.HHero.SetSwimSpeed();
        Client.HHero.SetRunSpeed();
        Client.HHero.SetGravity();
        Client.HHero.SetGlideSpeed();
        Client.HLevel.LevelBloomSettings.RevertToOriginal();
        Client.HHideSeek.CurrentPerk.Deactivate();
        Client.HHideSeek.CurrentPerk = new NoPerk();
        Client.HHideSeek.CurrentDebuff.Deactivate();
        Client.HHideSeek.CurrentDebuff = new NoPerk();

        ModelController.Lobby.IsReadyButtonEnabled = true;
        SFXPlayer.PlaySound(SFX.TAOpen);
    }

    [MessageHandler((ushort)MessageID.HS_Abort)]
    private static void Abort(Message message)
    {
        Client.HHideSeek.Role = HSRole.Seeker;
        Client.HHideSeek.Mode = HSMode.Neutral;
        Client.HHideSeek._timerRunning = false;
        Client.HHero.SetSwimSpeed();
        Client.HHero.SetRunSpeed();
        Client.HHero.SetGravity();
        Client.HHero.SetGlideSpeed();
        Client.HLevel.LevelBloomSettings.RevertToOriginal();
        Client.HHideSeek.CurrentPerk.Deactivate();
        Client.HHideSeek.CurrentPerk = new NoPerk();
        Client.HHideSeek.CurrentDebuff.Deactivate();
        Client.HHideSeek.CurrentDebuff = new NoPerk();
        ModelController.Lobby.IsReadyButtonEnabled = true;
        SFXPlayer.PlaySound(SFX.TAOpen);
    }

    public void StartTimerLoop()
    {
        var timer = new Thread(() => UpdateTime(Client.cts));
        timer.Start();
    }
    
    private void UpdateTime(CancellationTokenSource cts)
    {
        //REPLACE WITH TOKEN
        while (!cts.IsCancellationRequested && SettingsHandler.GameMode == GameMode.HideSeek)
        {
            if (!_timerRunning) 
                continue;
            Time++;
            Thread.Sleep(1000);
        }
    }
}

public enum HSRole
{
    Hider,
    Seeker,
    Spectator
}

public enum HSMode
{
    HideTime,
    SeekTime,
    Neutral
}