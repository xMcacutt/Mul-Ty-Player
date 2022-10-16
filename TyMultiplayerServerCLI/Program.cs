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
        ConsoleSend,
        ServerDataUpdate,
        ClientLevelDataUpdate,
        ClientAttributeDataUpdate
    }

    internal class Program
    {
        public static Server Server;
        private static bool _isRunning;
        public static Dictionary<ushort, Player> PlayerList;
        public static KoalaHandler KoalaHandler;
        public static CollectiblesHandler CollectiblesHandler;

        private static void Main()
        {
            Console.Title = "Mul-Ty-Player Server";

            SettingsHandler.Setup();

            RiptideLogger.Initialize(Console.WriteLine, true);
            _isRunning = true;

            new Thread(new ThreadStart(Loop)).Start();

            PlayerList = new Dictionary<ushort, Player>();
            KoalaHandler = new KoalaHandler();
            CollectiblesHandler = new CollectiblesHandler();
            Console.WriteLine("Welcome to Mul-Ty-Player.\nThis is the server application. \nPort forward on port 8750 to allow connections.\n");

            string command = Console.ReadLine();
            while(command != "/stop")
            {
                switch (command)
                {
                    
                }
                command = Console.ReadLine();
            }
            _isRunning = false;
        }

        private static void Loop()
        {
            Server = new Server(5000);
            Server.Start(8750, 8);

            Server.ClientConnected += (s, e) => ClientConnected();
            Server.ClientDisconnected += (s, e) => ClientDisconnected(s, e);

            while (_isRunning)
            {
                Server.Tick();
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

            Server.Stop();
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
                    Server.SendToAll(message);
                }
            }
        }

        public static void SendMessageToClients(string str, bool printToServer)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now}] (SERVER) {str}");
            Server.SendToAll(message);
            if (printToServer) { Console.WriteLine(str); }
        }

    }
}
