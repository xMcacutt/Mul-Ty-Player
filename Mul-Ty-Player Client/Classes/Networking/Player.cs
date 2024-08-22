using MulTyPlayerClient.Classes.Utility;
using PropertyChanged;

namespace MulTyPlayerClient;

[AddINotifyPropertyChangedInterface]
public class Player
{
    public ushort Id { get; set; }
    public Koala? Koala { get; set; }
    public string Name { get; set; }
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public HSRole Role { get; set; }
    public string Level { get; set; }
    public int Score { get; set; }
    public VIP VIP;

    public Player(Koala? koala, string name, ushort id, bool isHost, bool isReady, HSRole role, int score, VIP vip)
    {
        Koala = koala;
        Name = name;
        Id = id;
        IsHost = isHost;
        IsReady = isReady;
        Role = role;
        Score = score;
        VIP = vip;
    }
}