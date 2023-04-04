using MulTyPlayerClient.GUI;
using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Threading;

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
                "kick",
                "password",
                "clist",
                "restart"
            };
        }

        public void ParseCommand(string userInput)
        {
            if (userInput == null) { return; }

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
                        if (Host == Client._client.Id)
                        {
                            BasicIoC.LoggerInstance.Write("You already have host privileges");
                            break;
                        }
                        RequestHost();
                        break;
                    }
                case "help":
                    {
                        foreach (string line in File.ReadLines("./help.mtps"))
                        {
                            BasicIoC.LoggerInstance.Write(line);
                        }
                        break;
                    }
                case "msg":
                    {
                        if (args.Length == 0)
                        {
                            BasicIoC.LoggerInstance.Write("Usage: /msg [message]");
                            break;
                        }
                        string message = userInput[5..];
                        SendMessage(message, null);
                        BasicIoC.LoggerInstance.Write("Sent message to all connected clients.");
                        break;
                    }
                case "whisper":
                    {
                        if (args.Length < 2 || !ushort.TryParse(args[0], out _))
                        {
                            BasicIoC.LoggerInstance.Write("Usage: /whisper [client Id] [message]");
                            break;
                        }
                        if (!PlayerHandler.Players.ContainsKey(ushort.Parse(args[0])))
                        {
                            BasicIoC.LoggerInstance.Write($"{args[0]} is not a valid client ID");
                            break;
                        }
                        string message = userInput[(userInput.IndexOf(' ') + 1 + args[0].Length + 1)..];
                        SendMessage(message, ushort.Parse(args[0]));
                        BasicIoC.LoggerInstance.Write($"Sent message to client {ushort.Parse(args[0])}.");
                        break;
                    }
                default:
                    {
                        BasicIoC.LoggerInstance.Write($"/{command} is not a command. Try /help for a list of commands");
                        break;
                    }
            }
        }

        private static void RequestHost()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ReqHost);
            Client._client.Send(message);
        }

        private static void SendMessage(string text, ushort? toClientId)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.P2PMessage);
            message.AddString(text);
            //bSendToAll
            message.AddBool(toClientId == null);
            if(toClientId != null) message.AddUShort((ushort)toClientId);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReqHost)]
        public static void RequestHostResponse(Message message)
        {
            if (message.GetBool())
            {
                BasicIoC.LoggerInstance.Write("You have been made host. You now have access to host only commands.");
                Host = Client._client.Id;

                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                        new Action(BasicIoC.MainGUIViewModel.UpdateHostIcon));
                return;
            }
            BasicIoC.LoggerInstance.Write("Someone else who is connected already has host privileges");
        }

        [MessageHandler((ushort)MessageID.HostChange)]
        public static void HostChange(Message message)
        {
            Host = message.GetUShort();
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    new Action(BasicIoC.MainGUIViewModel.UpdateHostIcon));
        }

        [MessageHandler((ushort)MessageID.HostCommand)]
        public static void HostCommandResponse(Message message)
        {
            BasicIoC.LoggerInstance.Write(message.GetString());
        }

        [MessageHandler((ushort)MessageID.ResetSync)]
        private static void HandleSyncReset(Message message)
        {
            Client.HSync = new SyncHandler();
        }

        [MessageHandler((ushort)MessageID.P2PMessage)]
        private static void HandleMessageFromPeer(Message message)
        {
            BasicIoC.LoggerInstance.Write(message.GetString());
        }
    }
}
