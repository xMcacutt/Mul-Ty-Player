namespace MulTyPlayerClient
{
    internal class Player
    {
        public Koala Koala { get; set; }
        public string Name;
        public ushort Id;
        public bool IsHost;
        public bool IsReady;
        
        public Player(Koala koala, string name, ushort id, bool isHost, bool isReady)
        {
            Koala = koala;
            Name = name;
            Id = id;
            IsHost = isHost;
            IsReady = isReady;
        }
    }
}
