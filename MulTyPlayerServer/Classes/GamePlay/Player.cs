using Riptide;

namespace MulTyPlayerServer
{
    internal class Player
    {
        public ushort Id { get; private set; }
        public string Name { get; private set; }
        public Koala AssignedKoala;
        public int CurrentLevel;
        public int PreviousLevel = 99;

        public Player(ushort id, string name)
        {
            Id = id;
            Name = name;
        }

        /*MESSAGE HANDLING*/

        [MessageHandler((ushort)MessageID.Connected)]
        private static void HandleConnectedInitialization(ushort fromClientId, Message message)
        {
            string name = message.GetString();
            ushort id = fromClientId;
            Server.SendMessageToClients($"{name} joined the server.", true, fromClientId);
            Player player = new Player(id, name);
            Server.PlayerList.Add(id, player);
            Program.HKoala.AssignKoala(player);
            Server.SendMessageToClients($"{name} was assgined {player.AssignedKoala.Name}", true, fromClientId);
            Server.SendMessageToClient($"You were assigned {player.AssignedKoala.Name}", false, fromClientId);
        }

        [MessageHandler((ushort)MessageID.PlayerInfo)]
        private static void HandleGettingCoordinates(ushort fromClientId, Message message)
        {
            if (Server.PlayerList.ContainsKey(fromClientId))
            {
                if (message.GetBool()) { return; }
                Server.PlayerList[fromClientId].CurrentLevel = message.GetInt();
                Server.PlayerList[fromClientId].AssignedKoala.Coordinates = message.GetFloats();
            }
        }
    }
}
