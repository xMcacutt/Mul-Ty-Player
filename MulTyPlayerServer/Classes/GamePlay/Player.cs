using Riptide;

namespace MulTyPlayerServer
{
    internal class Player
    {
        public Koala Koala { get; set; }
        public string Name;
        public ushort ClientID;
        public float[] Coordinates;
        public int CurrentLevel;
        public int PreviousLevel = 99;

        public Player(Koala koala, string name, ushort id)
        {
            this.Koala = koala;
            this.Name = name;
            this.ClientID = id;
            Coordinates = new float[6];
        }
    }
}
