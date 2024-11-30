using PropertyChanged;


namespace Mul_Ty_Player_Updater.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SetupViewModel
{
    public bool UpdateClient { get; set; }
    public string ClientPath { get; set; }
    public bool UpdateServer { get; set; }
    public string ServerPath { get; set; }
    public bool UpdateGameFiles { get; set; }
    public string MTPPath { get; set; }
    public string Version { get; set; }
    public bool RemoveMagnetRandom { get; set; }
    public bool RevertOutbackMovement { get; set; }
    public bool RevertRangSwitching { get; set; }
    public bool FixControllerCameraAiming { get; set; }
    public bool OpenAllGameInfo { get; set; }
    public bool FixMenuBug { get; set; }
    
    public SetupViewModel()
    {
        var s = SettingsHandler.Settings;
        if (s == null) return;
        UpdateClient = s.UpdateClient;
        UpdateServer = s.UpdateServer;
        UpdateGameFiles = s.UpdateRKV;
        ClientPath = s.ClientDir;
        ServerPath = s.ServerDir;
        MTPPath = s.GameDir;
        Version = s.Version;
    }

    public void SaveSettings()
    {
        if (SettingsHandler.Settings == null) return;
        SettingsHandler.Settings.UpdateClient = UpdateClient;
        SettingsHandler.Settings.UpdateServer = UpdateServer;
        SettingsHandler.Settings.UpdateRKV = UpdateGameFiles;
        SettingsHandler.Settings.ClientDir = ClientPath;
        SettingsHandler.Settings.ServerDir = ServerPath;
        SettingsHandler.Settings.GameDir = MTPPath;
        SettingsHandler.Settings.Version = Version;
        SettingsHandler.SaveSettings();
    }
}