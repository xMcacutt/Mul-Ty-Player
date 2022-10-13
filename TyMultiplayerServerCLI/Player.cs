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
        public ushort Id { get; private set; }
        public string Name { get; private set; }
        public Koala AssignedKoala;
        public int CurrentLevel;
        public int PreviousLevel = 99;

        public Player(ushort id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /*MESSAGE HANDLING*/

        [MessageHandler((ushort)MessageID.Connected)]
        private static void HandleConnectedInitialization(ushort fromClientdId, Message message)
        {
            string name = message.GetString();
            ushort id = fromClientdId;
            Program.SendMessageToClients($"{name} joined the server.", true);
            Player player = new Player(id, name);
            Program.PlayerList.Add(id, player);
            Program.KoalaHandler.AssignKoala(player);
            Program.SendMessageToClients($"{name} was assgined {player.AssignedKoala.Name}", true);
        }

        [MessageHandler((ushort)MessageID.PlayerInfo)]
        private static void HandleGettingCoordinates(ushort fromClientdId, Message message)
        {
            if (Program.PlayerList.ContainsKey(fromClientdId))
            {
                if (message.GetBool()) { return; }
                Program.PlayerList[fromClientdId].CurrentLevel = message.GetInt();
                Program.PlayerList[fromClientdId].AssignedKoala.Coordinates = message.GetFloats();
            }
        }
    }
}
