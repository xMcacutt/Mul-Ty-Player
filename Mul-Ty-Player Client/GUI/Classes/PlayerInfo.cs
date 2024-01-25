using PropertyChanged;

namespace MulTyPlayerClient.GUI;

[AddINotifyPropertyChangedInterface]
public class PlayerInfo
{
    public PlayerInfo(ushort clientId, string playerName, string koalaName, HSRole role)
    {
        ClientId = clientId;
        PlayerName = playerName;
        KoalaName = koalaName;
        Role = role;
        IsHost = PlayerHandler.Players[clientId].IsHost;
        IsReady = PlayerHandler.Players[clientId].IsReady;
    }

    public ushort ClientId { get; set; }
    public string KoalaName { get; set; }
    public string PlayerName { get; set; }
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public HSRole Role { get; set; }
    public string Level { get; set; }
}

