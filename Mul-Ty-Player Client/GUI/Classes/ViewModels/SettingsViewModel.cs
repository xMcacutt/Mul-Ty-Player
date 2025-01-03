﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BenchmarkDotNet.Disassemblers;
using MulTyPlayerClient.Classes.Networking;
using NAudio.Wave;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SettingsViewModel
{
    public bool IsHost { get; set; }

    // CLIENT SETTINGS
    public string Theme { get; set; }
    
    public ObservableCollection<string> Themes { get; set; }
    public ObservableCollection<string> InputDevices { get; set; }
    
    public bool AutoRestartTy { get; set; }
    public bool DoGetSteamName { get; set; }
    public string DefaultName { get; set; }
    public bool AttemptReconnect { get; set; }
    public bool AutoLaunchTyOnStartup { get; set; }
    public bool DoChaosSeedLogging { get; set; }
    public bool DoAutoUpdate { get; set; }
    
    // GAMEPLAY SETTINGS
    public bool DoKoalaCollision { get; set; }
    public bool DoUseTyKoalaTextures { get; set; }
    public float KoalaScale { get; set; }
    public string InterpolationMode { get; set; }
    public bool ShowKoalaBeacons { get; set; }
    public int SelectedSlotIndex { get; set; }
    public bool DoOldOutbackMovement { get; set; }
    public bool DoOldRangSwap { get; set; }
    public bool DoControllerCameraAiming { get; set; }
    public bool DoFixMenuPositions { get; set; }
    public bool DoForceMagnets { get; set; }
    public bool DoUnlockGameInfo { get; set; }
    public ObservableCollection<string> SaveSlots { get; set; }
    public ObservableCollection<string> InterpolationModes { get; set; }
    
    
    // DEVELOPER SETTINGS
    public bool DoOutputLogs { get; set; }
    public ushort DefaultPort { get; set; }
    
    // VOICE SETTINGS
    public bool AutoJoinVoice { get; set; }
    public int ProximityRange { get; set; }
    public string SelectedInputDevice { get; set; }
    public float IgInputGain { get; set; }
    public float OgOutputGain { get; set; }
    public float CmpInputGain { get; set; }
    public float CmpThreshold { get; set; }
    public float CmpRatio { get; set; }
    public float CmpOutputGain { get; set; }
    public float NsGtFloor { get; set; }
    public float NsGtCeiling { get; set; }

    // SERVER SETTINGS
    public bool DoSyncTEs { get; set; }
    public bool DoSyncBilbies { get; set; }
    public bool DoSyncCogs { get; set; }
    public bool DoSyncFrames { get; set; }
    public bool DoSyncOpals { get; set; }
    public bool DoSyncScales { get; set; }
    public bool DoSyncRainbowCliffs { get; set; }
    public bool DoSyncRangs { get; set; }
    public bool DoSyncPortals { get; set; }
    public bool DoSyncMushrooms { get; set; }
    
    public void UpdateInputDevices()
    {
        InputDevices.Clear();
        for (var i = 0; i < WaveIn.DeviceCount; i++)
            InputDevices.Add(WaveIn.GetCapabilities(i).ProductName);
    }

    public void SetPropertiesFromSettings()
    {
        Themes = new ObservableCollection<string>();
        foreach (var theme in Directory.GetFiles("./GUI/Themes").Where(x =>
                     Path.GetExtension(x).Equals(".json", StringComparison.CurrentCultureIgnoreCase)))
            Themes.Add(Path.GetFileNameWithoutExtension(theme));

        InputDevices = new ObservableCollection<string>();
        UpdateInputDevices();

        InterpolationModes = new ObservableCollection<string>();
        foreach (var mode in Enum.GetNames(typeof(KoalaInterpolationMode)))
            InterpolationModes.Add(mode);
        
        SaveSlots = new ObservableCollection<string>() { "Save Slot 1", "Save Slot 2", "Save Slot 3" };
        
        AutoRestartTy = SettingsHandler.ClientSettings.AutoRestartTyOnCrash;
        Theme = SettingsHandler.ClientSettings.Theme;
        DoGetSteamName = SettingsHandler.ClientSettings.DoGetSteamName;
        DefaultName = SettingsHandler.ClientSettings.DefaultName;
        AttemptReconnect = SettingsHandler.ClientSettings.AttemptReconnect;
        AutoLaunchTyOnStartup = SettingsHandler.ClientSettings.AutoLaunchTyOnStartup;
        DoChaosSeedLogging = SettingsHandler.ClientSettings.DoLogChaosSeed;
        DoAutoUpdate = SettingsHandler.ClientSettings.DoAutoUpdate;
        
        DoKoalaCollision = SettingsHandler.ClientSettings.DoKoalaCollision;
        DoUseTyKoalaTextures = SettingsHandler.ClientSettings.UseTyKoalaTextures;
        KoalaScale = SettingsHandler.ClientSettings.KoalaScale;
        InterpolationMode = SettingsHandler.ClientSettings.InterpolationMode;
        ShowKoalaBeacons = SettingsHandler.ClientSettings.ShowKoalaBeacons;
        SelectedSlotIndex = SettingsHandler.ClientSettings.DefaultSaveSlot;
        DoOldOutbackMovement = SettingsHandler.ClientSettings.DoOldOutbackMovement;
        DoOldRangSwap = SettingsHandler.ClientSettings.DoOldRangSwap;
        DoControllerCameraAiming = SettingsHandler.ClientSettings.DoControllerCameraAiming;
        DoFixMenuPositions = SettingsHandler.ClientSettings.DoFixMenuPositions;
        DoForceMagnets = SettingsHandler.ClientSettings.DoForceMagnets;
        DoUnlockGameInfo = SettingsHandler.ClientSettings.DoUnlockGameInfo;

        AutoJoinVoice = SettingsHandler.ClientSettings.AutoJoinVoice;
        ProximityRange = SettingsHandler.ClientSettings.ProximityRange;
        SelectedInputDevice = InputDevices.FirstOrDefault(device => device == SettingsHandler.ClientSettings.VoiceInputDevice);
        IgInputGain = SettingsHandler.ClientSettings.IgGain;
        OgOutputGain = SettingsHandler.ClientSettings.OgGain;
        CmpInputGain = SettingsHandler.ClientSettings.CmpInputGain;
        CmpThreshold = SettingsHandler.ClientSettings.CmpThreshold;
        CmpRatio = SettingsHandler.ClientSettings.CmpRatio;
        CmpOutputGain = SettingsHandler.ClientSettings.CmpOutputGain;
        NsGtFloor = SettingsHandler.ClientSettings.NsGtFloor;
        NsGtCeiling = SettingsHandler.ClientSettings.NsGtCeiling;

        DoOutputLogs = SettingsHandler.ClientSettings.CreateLogFile;
        DefaultPort = SettingsHandler.ClientSettings.Port;

        DoSyncBilbies = SettingsHandler.DoBilbySyncing;
        DoSyncCogs = SettingsHandler.DoCogSyncing;
        DoSyncFrames = SettingsHandler.DoFrameSyncing;
        DoSyncOpals = SettingsHandler.DoOpalSyncing;
        DoSyncPortals = SettingsHandler.DoPortalSyncing;
        DoSyncRangs = SettingsHandler.DoRangSyncing;
        DoSyncScales = SettingsHandler.DoRainbowScaleSyncing;
        DoSyncTEs = SettingsHandler.DoTESyncing;
        DoSyncRainbowCliffs = SettingsHandler.DoCliffsSyncing;
        DoSyncMushrooms = SettingsHandler.DoMushroomSyncing;
    }

    public void SavePropertiesBackToSettings()
    {
        SettingsHandler.ClientSettings.AutoRestartTyOnCrash = AutoRestartTy;
        SettingsHandler.ClientSettings.Theme = Theme;
        SettingsHandler.ClientSettings.DoGetSteamName = DoGetSteamName;
        SettingsHandler.ClientSettings.DefaultName = DefaultName;
        SettingsHandler.ClientSettings.AttemptReconnect = AttemptReconnect;
        SettingsHandler.ClientSettings.AutoLaunchTyOnStartup = AutoLaunchTyOnStartup;
        SettingsHandler.ClientSettings.DoLogChaosSeed = DoChaosSeedLogging;
        SettingsHandler.ClientSettings.DoAutoUpdate = DoAutoUpdate;
        
        SettingsHandler.ClientSettings.DoKoalaCollision = DoKoalaCollision;
        SettingsHandler.ClientSettings.UseTyKoalaTextures = DoUseTyKoalaTextures;
        SettingsHandler.ClientSettings.KoalaScale = KoalaScale;
        SettingsHandler.ClientSettings.InterpolationMode = InterpolationMode;
        SettingsHandler.ClientSettings.ShowKoalaBeacons = ShowKoalaBeacons;
        SettingsHandler.ClientSettings.DefaultSaveSlot = SelectedSlotIndex;
        
        SettingsHandler.ClientSettings.DoOldOutbackMovement = DoOldOutbackMovement;
        SettingsHandler.ClientSettings.DoOldRangSwap = DoOldRangSwap;
        SettingsHandler.ClientSettings.DoControllerCameraAiming = DoControllerCameraAiming;
        SettingsHandler.ClientSettings.DoFixMenuPositions = DoFixMenuPositions;
        SettingsHandler.ClientSettings.DoForceMagnets = DoForceMagnets;
        SettingsHandler.ClientSettings.DoUnlockGameInfo = DoUnlockGameInfo;
        GameStateHandler.UpdateExeSettings();

        SettingsHandler.ClientSettings.AutoJoinVoice = AutoJoinVoice;
        SettingsHandler.ClientSettings.ProximityRange = ProximityRange;
        SettingsHandler.ClientSettings.VoiceInputDevice = SelectedInputDevice;
        SettingsHandler.ClientSettings.IgGain = IgInputGain;
        SettingsHandler.ClientSettings.OgGain = OgOutputGain;
        SettingsHandler.ClientSettings.CmpInputGain = CmpInputGain;
        SettingsHandler.ClientSettings.CmpThreshold = CmpThreshold;
        SettingsHandler.ClientSettings.CmpRatio = CmpRatio;
        SettingsHandler.ClientSettings.CmpOutputGain = CmpOutputGain;
        SettingsHandler.ClientSettings.NsGtFloor = NsGtFloor;
        SettingsHandler.ClientSettings.NsGtCeiling = NsGtCeiling;
        VoiceHandler.UpdateEffectsSettings();
        
        SettingsHandler.ClientSettings.CreateLogFile = DoOutputLogs;
        SettingsHandler.ClientSettings.Port = DefaultPort;
        
        if (IsHost)
        {
            SettingsHandler.DoBilbySyncing = DoSyncBilbies;
            SettingsHandler.DoCogSyncing = DoSyncCogs;
            SettingsHandler.DoFrameSyncing = DoSyncFrames;
            SettingsHandler.DoOpalSyncing = DoSyncOpals;
            SettingsHandler.DoPortalSyncing = DoSyncPortals;
            SettingsHandler.DoRangSyncing = DoSyncRangs;
            SettingsHandler.DoRainbowScaleSyncing = DoSyncScales;
            SettingsHandler.DoTESyncing = DoSyncTEs;
            SettingsHandler.DoCliffsSyncing = DoSyncRainbowCliffs;
            SettingsHandler.DoMushroomSyncing = DoSyncMushrooms;
            SettingsHandler.UpdateSyncSettings();
        }
        
        SettingsHandler.Save();

        App.AppColors.SetColors(SettingsHandler.ClientSettings.Theme);
        Client.HKoala.ScaleKoalas();
        Client.HKoala.SetCollision();
        Client.HGlow.ReturnGlows();
    }
    
    public void UpdateHost()
    {
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var player))
        {
            Logger.Write("[ERROR] Could not find self in player list.");
            return;
        }
        IsHost = player.IsHost;
    }
}