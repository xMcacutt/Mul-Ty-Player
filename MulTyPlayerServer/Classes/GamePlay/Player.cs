using Riptide;

namespace MulTyPlayerServer
{
    internal class Player
    {
        public Koala Koala { get; set; }
        public string Name;
        public ushort ClientID;
        public bool IsHost;
        public float[] Coordinates;
        public int CurrentLevel;
        public int PreviousLevel = 99;

        public Player(Koala koala, string name, ushort id, bool isHost)
        {
            Koala = koala;
            Name = name;
            ClientID = id;
            IsHost = isHost;
            Coordinates = new float[6];
        }
    }
}
