namespace MulTyPlayerServer;

public class ServerSettings
{
    public string Password { get; set; }
    public ushort Port { get; set; }
    public bool ResetPasswordOnEmpty { get; set; }
    public bool DoSyncTEs { get; set; }
    public bool DoSyncCogs { get; set; }
    public bool DoSyncBilbies { get; set; }
    public bool DoSyncRangs { get; set; }
    public bool DoSyncOpals { get; set; }
    public bool DoSyncPortals { get; set; }
    public bool DoSyncCliffs { get; set; }
    public bool DoSyncScale { get; set; }
    public bool DoSyncFrame { get; set; }
    public bool DoSyncMushrooms { get; set; }
    public bool DoAutoUpdate { get; set; }
    
    public string Version { get; set; }
}