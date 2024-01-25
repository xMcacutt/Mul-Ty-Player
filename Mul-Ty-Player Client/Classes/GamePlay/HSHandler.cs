using System;
using System.Linq;
using System.Numerics;
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
    public bool TimerRunning = false;
    private HSMode _mode = HSMode.Neutral;
    private bool _camLocked = false;
    
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
            if (_role == value) return;
            _role = value;
            if (value == HSRole.Seeker)
                TimerRunning = false;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)350));
            OnRoleChanged?.Invoke(value);
        }
    }
    public delegate void RoleChangedEventHandler(HSRole newRole);
    public static event RoleChangedEventHandler OnRoleChanged;
    
    public void Run()
    {
        
        if (_mode == HSMode.HideTime && Role == HSRole.Seeker)
        {
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)0));
            _camLocked = true;
            var pos = Client.HHero.GetCurrentPosRot();
            Client.HHero.WritePosition(pos[0], pos[1], pos[2], false);
            return;
        }
        
        if (_mode == HSMode.SeekTime && Role == HSRole.Hider)
        {
            if (!TimerRunning)
                TimerRunning = true;
            foreach (var player in ModelController.Lobby.PlayerInfoList.Where(x => x.Role == HSRole.Seeker))
            {
                if (!Enum.TryParse(typeof(Koala), player.KoalaName, out var koala))
                    continue;
                var seekerPos = PlayerReplication.PlayerTransforms[(int)koala].Position;
                var currentPos = Client.HHero.GetCurrentPosRot();
                var seekerVector = new Vector3(seekerPos.X, seekerPos.Y, seekerPos.Z);
                var currentVector = new Vector3(currentPos[0], currentPos[1], currentPos[2]);
                var distance = Vector3.Distance(seekerVector, currentVector);
                if (distance < 100)
                {
                    Client.HHero.Kill();
                    Client.HHideSeek.Role = HSRole.Seeker;
                }
            }
        }

        if (ModelController.Lobby.PlayerInfoList.Any(x => x.Role == HSRole.Hider))
            return;
        _mode = HSMode.Neutral;
        TimerRunning = false;
    }
    
    [MessageHandler((ushort)MessageID.HS_SetHideSeekMode)]
    private static void SetHideSeek(Message message)
    {
        SettingsHandler.DoHideSeek = message.GetBool();
        var result = SettingsHandler.DoHideSeek ? "Hide & Seek has been activated" : "Hide & Seek has been disabled";
        Logger.Write(result);
    }

    [MessageHandler((ushort)MessageID.HS_RoleChanged)]
    private static void PeerRoleChanged(Message message)
    {
        var clientId = message.GetUShort();
        var role = message.GetInt();
        if (!PlayerHandler.Players.TryGetValue(clientId, out var player))
            return;
        player.Role = (HSRole)role;
        var playerInfo = ModelController.Lobby.PlayerInfoList.FirstOrDefault(x => x.ClientId == clientId);
        if (playerInfo == null)
            return;
        playerInfo.Role = (HSRole)role;
    }

    [MessageHandler((ushort)MessageID.HS_HideTimerStart)]
    private static void HideTimerStart(Message message)
    {
        Client.HHideSeek._mode = HSMode.HideTime;
        foreach (var entry in PlayerHandler.Players) entry.Value.IsReady = false;
        ModelController.Lobby.IsReady = false;
        ModelController.Lobby.UpdateReadyStatus();
        SFXPlayer.PlaySound(SFX.HS_HideStart);
        if (Client.HHideSeek.Role == HSRole.Hider)
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)750));
        Logger.Write(Client.HHideSeek.Role == HSRole.Hider
            ? "[HIDE AND SEEK] You have 2 minutes to hide!"
            : "[HIDE AND SEEK] 2 minutes until seeking!");
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
        Client.HHideSeek._mode = HSMode.SeekTime;
        SFXPlayer.PlaySound(SFX.HS_SeekStart);
        if (Client.HHideSeek.Role == HSRole.Hider)
            Client.HHideSeek.TimerRunning = true;
        if (Client.HHideSeek.Role == HSRole.Seeker)
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EC00, BitConverter.GetBytes((float)750));
        Logger.Write(Client.HHideSeek.Role == HSRole.Hider
            ? $"[HIDE AND SEEK] The seekers are coming!"
            : $"[HIDE AND SEEK] Go find them!");
    }

    public void StartTimerLoop()
    {
        var timer = new Thread(() => UpdateTime(Client.cts));
        timer.Start();
    }
    
    private void UpdateTime(CancellationTokenSource cts)
    {
        //REPLACE WITH TOKEN
        while (!cts.IsCancellationRequested && SettingsHandler.DoHideSeek)
        {
            if (!TimerRunning) continue;
            Time++;
            Thread.Sleep(1000);
        }
    }
}

public enum HSRole
{
    Hider,
    Seeker
}

public enum HSMode
{
    HideTime,
    SeekTime,
    Neutral
}