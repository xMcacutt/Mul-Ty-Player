namespace MulTyPlayerClient;

internal class Settings
{
    public bool DoGetSteamName { get; set; }
    public string DefaultName { get; set; }
    public ushort Port { get; set; }
    public bool DoKoalaCollision { get; set; }
    public bool CreateLogFile { get; set; }
    public bool AttemptReconnect { get; set; }
    public bool AutoLaunchTyOnStartup { get; set; }
    public bool AutoRestartTyOnCrash { get; set; }
    public string MulTyPlayerFolderPath { get; set; }
    public string Theme { get; set; }
    public float KoalaScale { get; set; }
    public string InterpolationMode { get; set; }
    public bool AutoJoinVoice { get; set; }
    public int ProximityRange { get; set; }
    public string Version { get; set; }
    public bool UseTyKoalaTextures { get; set; }
}