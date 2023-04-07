using MulTyPlayerClient.GUI;
using Riptide;
using Riptide.Transports;
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

            if (PlayerHandler.Players[Client._client.Id].IsHost && hostCommands.Contains(command))
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
                        if (PlayerHandler.Players[Client._client.Id].IsHost)
                        {
                            BasicIoC.LoggerInstance.Write("You already have host privileges");
                            break;
                        }
                        RequestHost();
                        break;
                    }
                case "ready":
                    {
                        SetReady();
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

        public static bool HostExists()
        {
            return PlayerHandler.Players.Values.Any(p => p.IsHost);
        }

        private static void SendMessage(string text, ushort? toClientId)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.P2PMessage);
            message.AddBool(toClientId == null);
            message.AddString(text);
            if(toClientId != null) message.AddUShort((ushort)toClientId);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.Ready)]
        public static void PeerReady(Message message)
        {
            PlayerHandler.Players[message.GetUShort()].IsReady = message.GetBool();
        }

        public void SetReady()
        {
            PlayerHandler.Players[Client._client.Id].IsReady = !PlayerHandler.Players[Client._client.Id].IsReady;
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
            message.AddBool(PlayerHandler.Players[Client._client.Id].IsReady);
            Client._client.Send(message);
        }

        public void SetReady(ushort client)
        {
            PlayerHandler.Players[client].IsReady = !PlayerHandler.Players[client].IsReady;
        }

        [MessageHandler((ushort)MessageID.ReqHost)]
        public static void RequestHostResponse(Message message)
        {
            if (message.GetBool())
            {
                BasicIoC.LoggerInstance.Write("You have been made host. You now have access to host only commands.");
                PlayerHandler.Players[Client._client.Id].IsHost = true;

                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                        new Action(BasicIoC.MainGUIViewModel.UpdateHostIcon));
                return;
            }
            BasicIoC.LoggerInstance.Write($"Client {PlayerHandler.Players.Values.First(p => p.IsHost)} already has host privileges");
        }

        [MessageHandler((ushort)MessageID.HostChange)]
        public static void HostChange(Message message)
        {
            PlayerHandler.Players[message.GetUShort()].IsHost = true;
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
            BasicIoC.LoggerInstance.Write("Synchronisations have been reset to new game state.");
            Client.HSync = new SyncHandler();
        }

        [MessageHandler((ushort)MessageID.P2PMessage)]
        private static void HandleMessageFromPeer(Message message)
        {
            BasicIoC.LoggerInstance.Write(message.GetString());
        }
    }
}
