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

        public PlayerHandler()
        {
            Players = new();
        }

        public static void AddPlayer(string koalaName, string name, ushort clientID, bool isHost)
        {
            Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
            Players.Add(clientID, new Player(koala, name, clientID, isHost, false, true));
        }

        public static void RemovePlayer(ushort id)
        {
            Players.Remove(id);
        }

        public static void AnnounceDisconnect(ushort id)
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageID.AnnounceDisconnect);
            message.AddUShort(id);
            Server._Server.SendToAll(message);
        }

        [MessageHandler((ushort)MessageID.OnMenuStatus)]
        private static void HandleGettingOnMenuStatus(ushort fromClientId, Message message)
        {
            Players.TryGetValue(fromClientId, out Player player);
            if (player == null) return;
            player.OnMenu = true;
            return;
        }

        [MessageHandler((ushort)MessageID.PlayerInfo)]
        private static void HandleGettingCoordinates(ushort fromClientId, Message message)
        {
            if (Players.TryGetValue(fromClientId, out Player player))
            {
                player.OnMenu = message.GetBool();
                player.CurrentLevel = message.GetInt();
                player.Coordinates = message.GetFloats();
            }
        }
    }
}
