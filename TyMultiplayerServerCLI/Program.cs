using System;
using System.Threading;
using RiptideNetworking.Utils;
using RiptideNetworking;
using System.Collections.Generic;

namespace TyMultiplayerServerCLI
{
    public enum MessageID : ushort
    {
        Connected,
        PlayerInfo,
        KoalaCoordinates,
        ConsoleSend
    }

    internal class Program
    {
        private static Server _server;
        private static bool _isRunning;
        public static Dictionary<ushort, Player> PlayerList;
        public static KoalaHandler KoalaHandler;

        private static void Main()
        {
            Console.Title = "Mul-Ty-Player Server";

            SettingsHandler.Setup();

            RiptideLogger.Initialize(Console.WriteLine, true);
            _isRunning = true;

            new Thread(new ThreadStart(Loop)).Start();

            PlayerList = new Dictionary<ushort, Player>();
            KoalaHandler = new KoalaHandler();
            

            Console.WriteLine("Welcome to Mul-Ty-Player.\nThis is the server application. \nPort forward on port 8750 to allow connections.\n");
            Console.WriteLine("Press return to stop the server at any time.");

        }

        private static void Loop()
        {
            _server = new Server(5000);
            _server.Start(8750, 8);

            _server.ClientConnected += (s, e) => ClientConnected();
            _server.ClientDisconnected += (s, e) => ClientDisconnected(s, e);

            while (_isRunning)
            {
                _server.Tick();
                if (PlayerList.Count != 0)
                {
                    foreach (Player player in PlayerList.Values)
                    {
                        if (player.CurrentLevel != player.PreviousLevel)
                        {
                            KoalaHandler.ReturnKoala(player);
                            player.PreviousLevel = player.CurrentLevel;
                        }
                        SendCoordinates(player.AssignedKoala.KoalaId, player.CurrentLevel, player.AssignedKoala.Coordinates, player.Name);
                    }
                }
                Thread.Sleep(10);
            }

            _server.Stop();
        }

        private static void ClientConnected()
        {

        }

        private static void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            SendMessageToClients($"{PlayerList[e.Id].Name} has disconnected from the server.", true);
            SendMessageToClients($"{PlayerList[e.Id].AssignedKoala.Name} was returned to the koala pool", true);
            KoalaHandler.availableKoalas.Push(PlayerList[e.Id].AssignedKoala);
            KoalaHandler.ReturnKoala(PlayerList[e.Id]);
            PlayerList.Remove(e.Id);
        }

        public static void SendCoordinates(int koalaID, int level, float[] coordinates, string name)
        {
            foreach(Player player in PlayerList.Values)
            {
                Message message = Message.Create(MessageSendMode.unreliable, MessageID.KoalaCoordinates);
                int[] intData = { koalaID, level };
                message.AddInts(intData);
                message.AddFloats(coordinates);
                message.AddString(name);
                if(intData.Length == 2 && player.AssignedKoala.Coordinates != null && player.Name != null)
                { 
                    _server.SendToAll(message);
                }
            }
        }


        public static void SendMessageToClients(string str, bool printToServer)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now}] (SERVER) {str}");
            _server.SendToAll(message);
            if (printToServer) { Console.WriteLine(str); }
        }

    }
}
