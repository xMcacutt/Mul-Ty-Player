using PropertyChanged;

namespace MulTyPlayerClient.GUI;

[AddINotifyPropertyChangedInterface]
public class PlayerInfo
{
    public PlayerInfo(ushort clientID, string playerName, string koalaName)
    {
        ClientID = clientID;
        PlayerName = playerName;
        KoalaName = koalaName;
        IsHost = PlayerHandler.Players[clientID].IsHost;
        IsReady = PlayerHandler.Players[clientID].IsReady;
    }

    public ushort ClientID { get; set; }
    public string KoalaName { get; set; }
    public string PlayerName { get; set; }
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public string Level { get; set; }
}