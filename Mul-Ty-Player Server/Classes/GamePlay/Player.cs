namespace MulTyPlayerServer;

public class Player
{
    public ushort ClientID;
    public float[] Coordinates;
    public int CurrentLevel;
    public bool IsHost;
    public bool IsReady;
    public string Name;
    public bool OnMenu;
    public HSRole Role;
    public int Score;
    public int PreviousLevel = 99;

    public Player(Koala koala, string name, ushort id, bool isHost, bool isReady, bool onMenu, HSRole role)
    {
        Koala = koala;
        Name = name;
        ClientID = id;
        IsHost = isHost;
        Coordinates = new float[6];
        IsReady = isReady;
        OnMenu = onMenu;
        Role = role;
        Score = 0;
    }

    public Koala Koala { get; set; }
}