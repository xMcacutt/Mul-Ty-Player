﻿using System;
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
        public static bool _isRunning;

        static KoalaHandler HKoala => Program.HKoala;

        public static void StartServer()
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
            _isRunning = true;

            new Thread(new ThreadStart(Loop)).Start();
        }

        private static void Loop()
        {
            _Server = new Riptide.Server();
            _Server.Start(8750, 8);

            _Server.HandleConnection += (s, e) => HandleConnection(s, e);
            _Server.ClientConnected += (s, e) => ClientConnected(s, e);
            _Server.ClientDisconnected += (s, e) => ClientDisconnected(s, e);
            
            while (_isRunning)
            {
                _Server.Update();
                if (PlayerHandler.Players.Count != 0)
                {
                    foreach (Player player in PlayerHandler.Players.Values)
                    {
                        if (player.CurrentLevel != player.PreviousLevel)
                        {
                            HKoala.ReturnKoala(player);
                            player.PreviousLevel = player.CurrentLevel;
                        }
                        SendCoordinates(player.Koala.KoalaName, player.CurrentLevel, player.Coordinates);
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

        private static void HandleConnection(Connection pendingConnection, Message authenticationMessage)
        {
            string name = authenticationMessage.GetString();
            string pass = authenticationMessage.GetString();
            Console.WriteLine(pass);
            Console.WriteLine(SettingsHandler.Password);
            if (string.Equals(pass, SettingsHandler.Password, StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(SettingsHandler.Password, "XXXXX", StringComparison.CurrentCultureIgnoreCase)
                || string.IsNullOrWhiteSpace(SettingsHandler.Password))
            {
                _Server.Accept(pendingConnection);
            }
            else _Server.Reject(pendingConnection);
        }

        private static void ClientConnected(object sender, ServerConnectedEventArgs e)
        {
            if (_Server.Clients.Length == 1)
            {
                CommandHandler.SetNewHost(e.Client.Id);
            }
            KoalaHandler.SendKoalaAvailability(e.Client.Id);
            SettingsHandler.SendSettings(e.Client.Id);
        }

        private static void ClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Name} has disconnected from the server.", true);
            SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Koala.KoalaName} was returned to the koala pool", true);
            HKoala.ReturnKoala(PlayerHandler.Players[e.Client.Id]);
            PlayerHandler.Players.Remove(e.Client.Id);
        }

        public static void SendCoordinates(string koalaName, int level, float[] coordinates)
        {
            foreach (Player player in PlayerHandler.Players.Values)
            {
                Message message = Message.Create(MessageSendMode.Unreliable, MessageID.KoalaCoordinates);
                message.AddString(koalaName);
                message.AddInt(level);
                message.AddFloats(coordinates);
                if (player.Coordinates != null && player.Name != null)
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
