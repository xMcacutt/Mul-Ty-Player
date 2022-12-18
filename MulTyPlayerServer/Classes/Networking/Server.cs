using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Riptide;
using Riptide.Utils;

namespace MulTyPlayerServer
{
    internal class Server
    {
        public static Riptide.Server _Server;
        public static Dictionary<ushort, Player> PlayerList;
        public static bool _isRunning;

        static KoalaHandler HKoala => Program.HKoala;

        public static void StartServer()
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
            _isRunning = true;

            new Thread(new ThreadStart(Loop)).Start();

            PlayerList = new Dictionary<ushort, Player>();
        }

        private static void Loop()
        {
            _Server = new Riptide.Server();
            _Server.Start(8750, 8);

            _Server.ClientConnected += (s, e) => ClientConnected(s, e);
            _Server.ClientDisconnected += (s, e) => ClientDisconnected(s, e);
            
            while (_isRunning)
            {
                _Server.Update();
                if (PlayerList.Count != 0)
                {
                    foreach (Player player in PlayerList.Values)
                    {
                        if (player.CurrentLevel != player.PreviousLevel)
                        {
                            HKoala.ReturnKoala(player);
                            player.PreviousLevel = player.CurrentLevel;
                        }
                        SendCoordinates(player.AssignedKoala.KoalaId, player.CurrentLevel, player.AssignedKoala.Coordinates, player.Name);
                    }
                }
                Thread.Sleep(10);
            }

            if (Program._inputStr == "y") { return; }
            _Server.Stop();
            Console.WriteLine("Would you like to restart Mul-Ty-Player? [y/n]");
        }

        public static void RestartServer()
        {
            Program._inputStr = "y";
            _Server.Stop();
            _isRunning = false;
        }

        private static void ClientConnected(object sender, ServerConnectedEventArgs e)
        {
            if (_Server.Clients.Length == 1)
            {
                CommandHandler.SetNewHost(e.Client.Id);
            }
        }

        private static void ClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            SendMessageToClients($"{PlayerList[e.Client.Id].Name} has disconnected from the server.", true);
            SendMessageToClients($"{PlayerList[e.Client.Id].AssignedKoala.Name} was returned to the koala pool", true);
            KoalaHandler.availableKoalas.Push(PlayerList[e.Client.Id].AssignedKoala);
            HKoala.ReturnKoala(PlayerList[e.Client.Id]);
            PlayerList.Remove(e.Client.Id);
        }

        public static void SendCoordinates(int koalaID, int level, float[] coordinates, string name)
        {
            foreach (Player player in PlayerList.Values)
            {
                Message message = Message.Create(MessageSendMode.Unreliable, MessageID.KoalaCoordinates);
                message.AddInt(koalaID);
                message.AddInt(level);
                message.AddFloats(coordinates);
                message.AddString(name);
                if (player.AssignedKoala.Coordinates != null && player.Name != null)
                {
                    _Server.SendToAll(message);
                }
            }
        }

        public static void SendMessageToClient(string str, bool printToServer, ushort to)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now}] (SERVER) {str}");
            _Server.Send(message, to);
            if (printToServer) { Console.WriteLine(str); }
        }

        public static void SendMessageToClients(string str, bool printToServer, ushort except)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now}] (SERVER) {str}");
            _Server.SendToAll(message, except);
            if (printToServer) { Console.WriteLine(str); }
        }

        public static void SendMessageToClients(string str, bool printToServer)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now}] (SERVER) {str}");
            _Server.SendToAll(message);
            if (printToServer) { Console.WriteLine(str); }
        }
    }
}
