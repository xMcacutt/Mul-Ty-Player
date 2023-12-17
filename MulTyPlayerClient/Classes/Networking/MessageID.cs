namespace MulTyPlayerClient
{
    public enum MessageID : ushort
    {
        Connected,
        PlayerInfo,
        KoalaCoordinates,
        ConsoleSend,
        ServerDataUpdate,
        ClientDataUpdate,
        Disconnect,
        ResetSync,
        ReqSync,
        SyncSettings,
        ReqHost,
        HostChange,
        HostCommand,
        KoalaSelected,
        AnnounceDisconnect,
        P2PMessage,
        Ready,
        Countdown,
        StopWatch
    }
}
