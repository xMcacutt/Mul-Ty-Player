using System;
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
    
    // CLIENT SETTINGS
    public string Theme { get; set; }
    
    public ObservableCollection<string> Themes { get; set; }
    public ObservableCollection<string> InputDevices { get; set; }
    
    public bool AutoRestartTy { get; set; }
    public bool DoGetSteamName { get; set; }
    public string DefaultName { get; set; }
    public bool AttemptReconnect { get; set; }
    public bool AutoLaunchTyOnStartup { get; set; }
    
    // GAMEPLAY SETTINGS
    public bool DoKoalaCollision { get; set; }
    public bool DoUseTyKoalaTextures { get; set; }
    public float KoalaScale { get; set; }
    public string InterpolationMode { get; set; }
    public bool ShowKoalaBeacons { get; set; }
    
    public ObservableCollection<string> InterpolationModes { get; set; }
    
    // DEVELOPER SETTINGS
    public bool DoOutputLogs { get; set; }
    public ushort DefaultPort { get; set; }
    
    // VOICE SETTINGS
    public bool AutoJoinVoice { get; set; }
    public int ProximityRange { get; set; }


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
        
        AutoRestartTy = SettingsHandler.Settings.AutoRestartTyOnCrash;
        Theme = SettingsHandler.Settings.Theme;
        DoGetSteamName = SettingsHandler.Settings.DoGetSteamName;
        DefaultName = SettingsHandler.Settings.DefaultName;
        AttemptReconnect = SettingsHandler.Settings.AttemptReconnect;
        AutoLaunchTyOnStartup = SettingsHandler.Settings.AutoLaunchTyOnStartup;
        
        DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
        DoUseTyKoalaTextures = SettingsHandler.Settings.UseTyKoalaTextures;
        KoalaScale = SettingsHandler.Settings.KoalaScale;
        InterpolationMode = SettingsHandler.Settings.InterpolationMode;
        ShowKoalaBeacons = SettingsHandler.Settings.ShowKoalaBeacons;

        AutoJoinVoice = SettingsHandler.Settings.AutoJoinVoice;
        ProximityRange = SettingsHandler.Settings.ProximityRange;

        DoOutputLogs = SettingsHandler.Settings.CreateLogFile;
        DefaultPort = SettingsHandler.Settings.Port;
    }

    public void SavePropertiesBackToSettings()
    {
        SettingsHandler.Settings.AutoRestartTyOnCrash = AutoRestartTy;
        SettingsHandler.Settings.Theme = Theme;
        SettingsHandler.Settings.DoGetSteamName = DoGetSteamName;
        SettingsHandler.Settings.DefaultName = DefaultName;
        SettingsHandler.Settings.AttemptReconnect = AttemptReconnect;
        SettingsHandler.Settings.AutoLaunchTyOnStartup = AutoLaunchTyOnStartup;
        
        SettingsHandler.Settings.DoKoalaCollision = DoKoalaCollision;
        SettingsHandler.Settings.UseTyKoalaTextures = DoUseTyKoalaTextures;
        SettingsHandler.Settings.KoalaScale = KoalaScale;
        SettingsHandler.Settings.InterpolationMode = InterpolationMode;
        SettingsHandler.Settings.ShowKoalaBeacons = ShowKoalaBeacons;

        SettingsHandler.Settings.AutoJoinVoice = AutoJoinVoice;
        SettingsHandler.Settings.ProximityRange = ProximityRange;

        SettingsHandler.Settings.CreateLogFile = DoOutputLogs;
        SettingsHandler.Settings.Port = DefaultPort;
        
        
        SettingsHandler.Save();

        App.AppColors.SetColors(SettingsHandler.Settings.Theme);
        Client.HKoala.ScaleKoalas();
        Client.HKoala.SetCollision();
        Client.HGlow.ReturnGlows();
    }
}