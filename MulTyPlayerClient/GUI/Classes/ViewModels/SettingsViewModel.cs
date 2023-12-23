using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MulTyPlayerClient.Classes.Networking;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SettingsViewModel
{
    
    // CLIENT SETTINGS
    public string Theme { get; set; }
    
    public ObservableCollection<string> Themes { get; set; }
    
    public bool AutoRestartTy { get; set; }
    public bool DoCollectibleTracking { get; set; }
    public bool DoGetSteamName { get; set; }
    public string DefaultName { get; set; }
    public bool AttemptReconnect { get; set; }
    public bool AutoLaunchTyOnStartup { get; set; }
    
    // GAMEPLAY SETTINGS
    public bool DoKoalaCollision { get; set; }
    public float KoalaScale { get; set; }
    public string InterpolationMode { get; set; }
    
    public ObservableCollection<string> InterpolationModes { get; set; }
    
    // DEVELOPER SETTINGS
    public bool DoOutputLogs { get; set; }
    public ushort DefaultPort { get; set; }

    public void SetPropertiesFromSettings()
    {
        Themes = new ObservableCollection<string>();
        foreach (var theme in Directory.GetFiles("./GUI/Themes").Where(x =>
                     Path.GetExtension(x).Equals(".json", StringComparison.CurrentCultureIgnoreCase)))
            Themes.Add(Path.GetFileNameWithoutExtension(theme));

        InterpolationModes = new ObservableCollection<string>();
        foreach (var mode in Enum.GetNames(typeof(KoalaInterpolationMode)))
            InterpolationModes.Add(mode);
        
        AutoRestartTy = SettingsHandler.Settings.AutoRestartTyOnCrash;
        Theme = SettingsHandler.Settings.Theme;
        DoCollectibleTracking = SettingsHandler.Settings.DoCollectibleTracking;
        DoGetSteamName = SettingsHandler.Settings.DoGetSteamName;
        DefaultName = SettingsHandler.Settings.DefaultName;
        AttemptReconnect = SettingsHandler.Settings.AttemptReconnect;
        AutoLaunchTyOnStartup = SettingsHandler.Settings.AutoLaunchTyOnStartup;
        
        DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
        KoalaScale = SettingsHandler.Settings.KoalaScale;
        InterpolationMode = SettingsHandler.Settings.InterpolationMode;

        DoOutputLogs = SettingsHandler.Settings.CreateLogFile;
        DefaultPort = SettingsHandler.Settings.Port;
    }

    public void SavePropertiesBackToSettings()
    {
        SettingsHandler.Settings.AutoRestartTyOnCrash = AutoRestartTy;
        SettingsHandler.Settings.Theme = Theme;
        SettingsHandler.Settings.DoCollectibleTracking = DoCollectibleTracking;
        SettingsHandler.Settings.DoGetSteamName = DoGetSteamName;
        SettingsHandler.Settings.DefaultName = DefaultName;
        SettingsHandler.Settings.AttemptReconnect = AttemptReconnect;
        SettingsHandler.Settings.AutoLaunchTyOnStartup = AutoLaunchTyOnStartup;
        
        SettingsHandler.Settings.DoKoalaCollision = DoKoalaCollision;
        SettingsHandler.Settings.KoalaScale = KoalaScale;
        SettingsHandler.Settings.InterpolationMode = InterpolationMode;

        SettingsHandler.Settings.CreateLogFile = DoOutputLogs;
        SettingsHandler.Settings.Port = DefaultPort;
        
        SettingsHandler.Save();

        App.AppColors.SetColors(SettingsHandler.Settings.Theme);
    }
}