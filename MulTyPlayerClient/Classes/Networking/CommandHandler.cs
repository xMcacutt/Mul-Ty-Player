using MulTyPlayerClient.GUI;
using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MulTyPlayerClient
{
    internal class CommandHandler
    {
        public static ushort Host = 0;
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

            if (Host == Client._client.Id && hostCommands.Contains(command))
            {
                Message message = Message.Create(MessageSendMode.Reliable, MessageID.HostCommand);
                message.AddString(userInput);
                Client._client.Send(message);
                return;
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
                        foreach (string line in File.ReadLines("./help.mtps"))
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
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ReqHost);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReqHost)]
        public static void RequestHostResponse(Message message)
        {
            if (message.GetBool())
            {
                Console.WriteLine("You have been made host. You now have access to host only commands.");
                Host = Client._client.Id;
                return;
            }
            Console.WriteLine("Someone else who is connected already has host privileges");
        }

        [MessageHandler((ushort)MessageID.HostChange)]
        public static void HostChange(Message message)
        {
            Host = message.GetUShort();
            BasicIoC.MainGUIViewModel.PlayerInfoList.FirstOrDefault(playerInfo => playerInfo.ClientID == Host).IsHost = true;
        }

        [MessageHandler((ushort)MessageID.HostCommand)]
        public static void HostCommandResponse(Message message)
        {
            Console.WriteLine(message.GetString());
        }

        [MessageHandler((ushort)MessageID.ResetSync)]
        private static void HandleSyncReset(Message message)
        {
            Client.HSync = new SyncHandler();
        }
    }
}
