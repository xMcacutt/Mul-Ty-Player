using PropertyChanged;

namespace MulTyPlayerClient;

[AddINotifyPropertyChangedInterface]
internal class Player
{
    public ushort Id { get; set; }
    public Koala? Koala { get; set; }
    public string Name { get; set; }
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public HSRole? Role { get; set; }
    
    public string Level { get; set; }
    
    public Player(Koala? koala, string name, ushort id, bool isHost, bool isReady, HSRole? role)
    {
        Koala = koala;
        Name = name;
        Id = id;
        IsHost = isHost;
        IsReady = isReady;
        Role = role;
    }
}