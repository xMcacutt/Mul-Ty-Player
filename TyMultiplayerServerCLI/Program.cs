using System;
using System.Threading;
using RiptideNetworking.Utils;
using RiptideNetworking;
using System.Collections.Generic;

namespace TyMultiplayerServerCLI
{
    public enum MessageID : ushort
    {
        connected,
        playerinfo,
        koalacoordinates,
        consolesend
    }

    internal class Program
    {
        private static Server server;
        private static bool isRunning;
        public static Dictionary<ushort, Player> playerList;
        public static KoalaHandler koalaHandler;

        private static void Main()
        {
            Console.Title = "Mul-Ty-Player Server";
            RiptideLogger.Initialize(Console.WriteLine, true);
            isRunning = true;

            new Thread(new ThreadStart(Loop)).Start();

            playerList = new Dictionary<ushort, Player>();
            koalaHandler = new KoalaHandler();
            

            Console.WriteLine("Welcome to Mul-Ty-Player.\nThis is the server application. \nPort forward on port 8750 to allow connections.\n");
            Console.WriteLine("Press return to stop the server at any time.");

        }

        private static void Loop()
        {
            server = new Server(5000);
            server.Start(8750, 8);

            server.ClientConnected += (s, e) => ClientConnected();
            server.ClientDisconnected += (s, e) => ClientDisconnected(s, e);

            while (isRunning)
            {
                server.Tick();
                if (playerList.Count != 0)
                {
                    foreach (Player player in playerList.Values)
                    {
                        if (player.currentLevel != player.previousLevel)
                        {
                            koalaHandler.ReturnKoala(player);
                            player.previousLevel = player.currentLevel;
                        }
                        SendCoordinates(player.assignedKoala.koalaID, player.currentLevel, player.assignedKoala.coordinates, player.name);
                    }
                }
                Thread.Sleep(10);
            }

            server.Stop();
        }

        private static void ClientConnected()
        {

        }

        private static void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            SendMessageToClients($"{playerList[e.Id].name} has disconnected from the server.", true);
            SendMessageToClients($"{playerList[e.Id].assignedKoala.name} was returned to the koala pool", true);
            KoalaHandler.availableKoalas.Push(playerList[e.Id].assignedKoala);
            koalaHandler.ReturnKoala(playerList[e.Id]);
            playerList.Remove(e.Id);
        }

        public static void SendCoordinates(int koalaID, int level, float[] coordinates, string name)
        {
            foreach(Player player in playerList.Values)
            {
                Message message = Message.Create(MessageSendMode.unreliable, MessageID.koalacoordinates);
                int[] intData = { koalaID, level };
                message.AddInts(intData);
                message.AddFloats(coordinates);
                message.AddString(name);
                if(intData.Length == 2 && player.assignedKoala.coordinates != null && player.name != null)
                { 
                    server.SendToAll(message);
                }
            }
        }


        public static void SendMessageToClients(string str, bool printToServer)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.consolesend);
            message.AddString($"[{DateTime.Now}] (SERVER) {str}");
            server.SendToAll(message);
            if (printToServer) { Console.WriteLine(str); }
        }

    }
}
