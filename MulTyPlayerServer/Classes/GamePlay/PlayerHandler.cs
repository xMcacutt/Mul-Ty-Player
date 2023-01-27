using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class PlayerHandler
    {
        public static Dictionary<ushort, Player> Players = new();

        PlayerHandler()
        {
            Players = new();
        }

        public static void AddPlayer(string koalaName, string name, ushort clientID)
        {
            Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
            Players.Add(clientID, new Player(koala, name, clientID));
        }

        public static void RemovePlayer(ushort id)
        {
            Players.Remove(id);
        }

        [MessageHandler((ushort)MessageID.PlayerInfo)]
        private static void HandleGettingCoordinates(ushort fromClientId, Message message)
        {
            if (Players.ContainsKey(fromClientId))
            {
                if (message.GetBool()) { return; }
                Players[fromClientId].CurrentLevel = message.GetInt();
                Players[fromClientId].Coordinates = message.GetFloats();
            }
        }
    }
}
