using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MulTyPlayerClient
{
    internal class CommandHandler
    {
        public static ushort host;
        public List<string> hostCommands;

        public CommandHandler()
        {
            hostCommands = new List<string>
            {
                "resetsync",
                "msg",
                "kick",
                "password",
                "clist",
                "restart"
            };
        }

        public void ParseCommand(string userInput)
        {
            if (userInput == null) { return; }

            if (!userInput.StartsWith("/")) { return; }

            string command = userInput.Split(' ')[0].Trim('/');
            string[] args = userInput.Split(' ').Skip(1).ToArray();



            if (host == Client._client.Id && hostCommands.Contains(command))
            {
                Message message = Message.Create(MessageSendMode.reliable, MessageID.HostCommand);
                message.AddString(userInput);
                Client._client.Send(message);
            }

            switch (command)
            {
                case "requesthost":
                    {
                        RequestHost();
                        break;
                    }
                case "help":
                    {
                        foreach (string line in File.ReadLines("../help.mtps"))
                        {
                            Console.WriteLine(line);
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"/{command} is not a command. Try /help for a list of commands");
                        break;
                    }
            }
        }

        private static void RequestHost()
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ReqHost);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReqHost)]
        public static void RequestHostResponse(Message message)
        {
            if (message.GetBool())
            {
                Console.WriteLine("You have been made host. You now have access to host only commands.");
                host = Client._client.Id;
                return;
            }
            Console.WriteLine("Someone else who is connected already has host privileges");
        }

        [MessageHandler((ushort)MessageID.HostChange)]
        public static void HostChange(Message message)
        {
            host = message.GetUShort();
        }
    }
}
