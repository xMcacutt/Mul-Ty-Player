namespace MulTyPlayerServer
{
    internal class Player
    {
        public Koala Koala { get; set; }
        public string Name;
        public ushort ClientID;
        public bool IsHost;
        public bool IsReady;
        public bool OnMenu;
        public float[] Coordinates;
        public int CurrentLevel;
        public int PreviousLevel = 99;

        public Player(Koala koala, string name, ushort id, bool isHost, bool isReady, bool onMenu)
        {
            Koala = koala;
            Name = name;
            ClientID = id;
            IsHost = isHost;
            Coordinates = new float[6];
            IsReady = isReady;
            OnMenu = onMenu;
        }
    }
}
