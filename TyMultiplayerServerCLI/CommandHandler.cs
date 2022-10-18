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

namespace TyMultiplayerServerCLI
{
    internal class CommandHandler
    {
        public void ParseCommand(string userInput)
        {
            if (userInput == null) { return; }

            if (!userInput.StartsWith("/")) { return; }

            string command = userInput.Split(' ')[0].Trim('/');
            string[] args = userInput.Split(' ').Skip(1).ToArray();

            switch (command)
            {
                case "resetsync":
                    {
                        ResetSync();
                        break;
                    }
                case "kick":
                    {
                        if(args.Length == 0)
                        {
                            Console.WriteLine("Usage: /kick [client id]\nFor a list of client ids, use /clist");
                        }
                        ushort res;
                        if (!ushort.TryParse(args[0], out res))
                        {
                            Console.WriteLine(res + "is not a valid client ID");
                        }
                        KickPlayer(ushort.Parse(args[0]));
                        break;
                    }
                case "clist":
                    {
                        ListClients();
                        break;
                    }
                case "password":
                    {
                        if(args.Length == 0)
                        {
                            Console.WriteLine($"The current password is {SettingsHandler.Password}");
                            Console.WriteLine("To set a new password use /password [password]");
                            break;
                        }
                        SetPassword(args[0]);
                        break;
                    }
                case "help":
                    {
                        foreach(string line in File.ReadLines("./help.mtps"))
                        {
                            Console.WriteLine(line);
                        }
                        break;
                    }
                case "msg":
                    {
                        if (args.Length == 0)
                        {
                            Console.WriteLine("Usage: /msg [message]");
                            return;
                        }
                        string message = "";
                        foreach(string s in args)
                        {
                            message += s;
                            message += " ";
                        }
                        Program.SendMessageToClients(message, false);
                        Console.WriteLine("Sent message to all connected clients");
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"/{command} is not a command. Try /help for a list of commands");
                        break;
                    }
            }
        }

        private void ResetSync()
        {

        }

        private void KickPlayer(ushort clientId)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.Disconnect);
            Program.Server.Send(message, clientId);
        }

        private void ListClients()
        {
            Console.WriteLine("\n--------------- Connected Clients ---------------");
            if(Program.PlayerList.Count == 0)
            {
                Console.WriteLine("There are no clients connected");
                return;
            }
            foreach (IConnectionInfo client in Program.Server.Clients)
            {
                if (client.IsConnected)
                {
                    Console.WriteLine("Client " + client.Id + " Name: " + Program.PlayerList[client.Id].Name);
                }
            }
        }

        private void SetPassword(string password)
        {
            if (!password.All(Char.IsLetter) || password.Length != 5)
            {
                Console.WriteLine($"{password} is not a valid password. A password must be exactly 5 LETTERS.");
                return;
            }
            SettingsHandler.Password = password.ToUpper();
            Console.WriteLine($"{password.ToUpper()} set as new password");
        }
    }
}
