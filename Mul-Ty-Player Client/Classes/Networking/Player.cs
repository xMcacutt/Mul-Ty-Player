namespace MulTyPlayerClient;

internal class Player
{
    public ushort Id;
    public bool IsHost;
    public bool IsReady;
    public string Name;
    public HSRole Role;
    public Koala Koala;

    public Player(Koala koala, string name, ushort id, bool isHost, bool isReady, HSRole role)
    {
        Koala = koala;
        Name = name;
        Id = id;
        IsHost = isHost;
        IsReady = isReady;
        Role = role;
    }

}