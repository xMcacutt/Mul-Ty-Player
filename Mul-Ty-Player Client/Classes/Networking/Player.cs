namespace MulTyPlayerClient;

internal class Player
{
    public ushort Id;
    public bool IsHost;
    public bool IsReady;
    public string Name;

    public Player(Koala koala, string name, ushort id, bool isHost, bool isReady)
    {
        Koala = koala;
        Name = name;
        Id = id;
        IsHost = isHost;
        IsReady = isReady;
    }

    public Koala Koala { get; set; }
}