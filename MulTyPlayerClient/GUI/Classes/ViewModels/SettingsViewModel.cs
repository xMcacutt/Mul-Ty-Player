using MulTyPlayerClient.Classes.Networking;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SettingsViewModel
{
    
    // CLIENT SETTINGS
    public bool AutoRestartTy { get; set; }
    public bool DarkMode { get; set; }
    public bool DoCollectibleTracking { get; set; }
    public bool DoGetSteamName { get; set; }
    public string DefaultName { get; set; }
    public bool AttemptReconnect { get; set; }
    public bool AutoLaunchTyOnStartup { get; set; }
    
    // GAMEPLAY SETTINGS
    public bool DoKoalaCollision { get; set; }
    public float KoalaScale { get; set; }
    public string KoalaInterpolationMode { get; set; }
    
    // DEVELOPER SETTINGS
    public bool DoOutputLogs { get; set; }
    public int DefaultPort { get; set; }

    public void SetPropertiesFromSettings()
    {
        AutoRestartTy = SettingsHandler.Settings.AutoRestartTyOnCrash;
        DarkMode = SettingsHandler.Settings.DarkMode;
        DoCollectibleTracking = SettingsHandler.Settings.DoCollectibleTracking;
        DoGetSteamName = SettingsHandler.Settings.DoGetSteamName;
        DefaultName = SettingsHandler.Settings.DefaultName;
        AttemptReconnect = SettingsHandler.Settings.AttemptReconnect;
        AutoLaunchTyOnStartup = SettingsHandler.Settings.AutoLaunchTyOnStartup;
        
        DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
        KoalaScale = SettingsHandler.Settings.KoalaScale;
        KoalaInterpolationMode = SettingsHandler.Settings.InterpolationMode;

        DoOutputLogs = SettingsHandler.Settings.CreateLogFile;
        DefaultPort = SettingsHandler.Settings.Port;
    }
}