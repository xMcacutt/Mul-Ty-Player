using System.IO;
using Newtonsoft.Json;

namespace Mul_Ty_Player_Updater;

public class SettingsHandler
{
    public static Settings? Settings;

    public SettingsHandler()
    {
       LoadSettings(); 
    }

    public static void LoadSettings()
    {
        var json = File.ReadAllText("./Settings.json");
        Settings = JsonConvert.DeserializeObject<Settings>(json);
    }

    public static void SaveSettings()
    {
        var json = JsonConvert.SerializeObject(Settings);
        File.WriteAllText("./Settings.json", json);
    }
}

public class Settings
{
    public bool UpdateClient { get; set; }
    public bool UpdateServer { get; set; }
    public bool InstallMiniUpdaterClient { get; set; }
    public bool InstallMiniUpdaterServer { get; set; }
    public bool UpdateRKV { get; set; }
    public string ClientDir { get; set; }
    public string ServerDir { get; set; }
    public string GameDir { get; set; }
    public bool RevertOutbackMovement { get; set; }
    public bool RevertRangSwitching { get; set; }
    public bool FixControllerCameraAiming { get; set; }
    public bool OpenAllGameInfo { get; set; }
    public bool FixMenuBug { get; set; }
    public bool FixedMagnets { get; set; }
    public string Version { get; set; }
}