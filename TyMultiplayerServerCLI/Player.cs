using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiptideNetworking;
using RiptideNetworking.Utils;

namespace TyMultiplayerServerCLI
{
    internal class Player
    {
        public ushort id { get; private set; }
        public string name { get; private set; }
        public Koala assignedKoala;
        public int currentLevel;
        public int previousLevel = 99;

        public Player(ushort id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /*MESSAGE HANDLING*/

        [MessageHandler((ushort)MessageID.connected)]
        private static void HandleConnectedInitialization(ushort fromClientdId, Message message)
        {
            string name = message.GetString();
            ushort id = fromClientdId;
            Program.SendMessageToClients($"{name} joined the server.", true);
            Player player = new Player(id, name);
            Program.playerList.Add(id, player);
            Program.koalaHandler.AssignKoala(player);
            Program.SendMessageToClients($"{name} was assgined {player.assignedKoala.name}", true);
        }

        [MessageHandler((ushort)MessageID.playerinfo)]
        private static void HandleGettingCoordinates(ushort fromClientdId, Message message)
        {
            if (Program.playerList.ContainsKey(fromClientdId))
            {
                if (message.GetBool()) { return; }
                Program.playerList[fromClientdId].currentLevel = message.GetInt();
                Program.playerList[fromClientdId].assignedKoala.coordinates = message.GetFloats();
            }
        }
    }
}
