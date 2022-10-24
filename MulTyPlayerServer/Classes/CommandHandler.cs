using Microsoft.SqlServer.Server;
using RiptideNetworking.Transports;
using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace MulTyPlayerServer
{
    internal class CommandHandler
    {
        public static ushort host;

        public string ParseCommand(string userInput)
        {
            if (userInput == null) { return null; }

            if (!userInput.StartsWith("/")) { return null; }

            string command = userInput.Split(' ')[0].Trim('/');
            string[] args = userInput.Split(' ').Skip(1).ToArray();

            switch (command)
            {
                case "resetsync":
                    {
                        ResetSync();
                        return "Synchronizations have been reset.";
                    }
                case "kick":
                    {
                        if (args.Length == 0)
                        {
                            return "Usage: /kick [client id]\nFor a list of client ids, use /clist";
                        }
                        ushort res;
                        if (!ushort.TryParse(args[0], out res) || !Server.PlayerList.ContainsKey(ushort.Parse(args[0])))
                        {
                            return args[0] + " is not a valid client ID";
                        }
                        string kickSuccess = $"Successfully Kicked {Server.PlayerList[ushort.Parse(args[0])].Name}";
                        KickPlayer(ushort.Parse(args[0]));
                        return kickSuccess;
                    }
                case "clist":
                    {
                        return ListClients();
                    }
                case "password":
                    {
                        if (args.Length == 0)
                        {
                            return $"The current password is {SettingsHandler.Password}\nTo set a new password use /password [password]";
                        }
                        return SetPassword(args[0]);
                    }
                case "help":
                    {
                        string help = null;
                        foreach (string line in File.ReadLines("./help.mtps"))
                        {
                            help += line;
                            help += "\n";
                        }
                        return help;
                    }
                case "msg":
                    {
                        if (args.Length == 0)
                        {
                            return "Usage: /msg [message]";
                        }
                        string message = "";
                        foreach (string s in args)
                        {
                            message += s;
                            message += " ";
                        }
                        Server.SendMessageToClients(message, false);
                        return "Sent message to all connected clients.";
                    }
                case "restart":
                    {
                        Server.RestartServer();
                        return null;
                    }
                default:
                    {
                        return $"/{command} is not a command. Try /help for a list of commands";
                    }
            }
        }

        private void ResetSync()
        {
            Program.CollectiblesHandler = new CollectiblesHandler();
            CollectiblesHandler.SendResetSyncMessage();
        }

        private void KickPlayer(ushort clientId)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.Disconnect);
            Server._Server.Send(message, clientId);
        }

        private string ListClients()
        {
            string listRes = null;
            listRes += "\n--------------- Connected Clients ---------------";
            if (Server.PlayerList.Count == 0)
            {
                return listRes += "\nThere are no clients currently connected.";
            }
            foreach (IConnectionInfo client in Server._Server.Clients)
            {
                if (client.IsConnected)
                {
                    return listRes += "Client " + client.Id + " Name: " + Server.PlayerList[client.Id].Name;
                }
            }
            return listRes;
        }

        private string SetPassword(string password)
        {
            if (!password.All(Char.IsLetter) || password.Length != 5)
            {
                return $"{password} is not a valid password. A password must be exactly 5 LETTERS.";
            }
            SettingsHandler.Password = password.ToUpper();
            return $"{SettingsHandler.Password} set as new password.";
        }

        [MessageHandler((ushort)MessageID.ReqHost)]
        public static void RequestHost(ushort fromClientId, Message message)
        {
            bool acceptRequest = false;
            if (!Server._Server.Clients[host].IsConnected)
            {
                acceptRequest = true;
                SetNewHost(fromClientId);
            }
            Message hRequest = Message.Create(MessageSendMode.reliable, MessageID.ReqHost);
            hRequest.AddBool(acceptRequest);
            Server._Server.Send(hRequest, fromClientId);
        }

        public static void SetNewHost(ushort newHost)
        {
            host = newHost;
            Message notifyHostChange = Message.Create(MessageSendMode.reliable, MessageID.HostChange);
            notifyHostChange.AddUShort(newHost);
            Server._Server.SendToAll(notifyHostChange);
            if(Server.PlayerList.Count == 0)
            {
                return;
            }
            Server.SendMessageToClients($"{Server.PlayerList[newHost].Name} has been made host", true);
        }

        [MessageHandler((ushort)MessageID.HostCommand)]
        public static void HostCommand(ushort fromClientId, Message message)
        {
            Message response = Message.Create(MessageSendMode.reliable, MessageID.HostCommand);
            response.AddString(Program.CommandHandler.ParseCommand(message.GetString()));
            Server._Server.Send(message, fromClientId);
        }
    }
}
