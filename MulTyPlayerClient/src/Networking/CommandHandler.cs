using MulTyPlayerClient.GUI;
using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayerCommon;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.Networking
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
                "clist"
            };
        }

        public void ParseCommand(string userInput)
        {
            if (userInput == null) { return; }

            string command = userInput.Split(' ')[0].Trim('/');
            string[] args = userInput.Split(' ').Skip(1).ToArray();

            if (PlayerHandler.Players[ConnectionService.Client.Id].IsHost && hostCommands.Contains(command))
            {
                Message message = Message.Create(MessageSendMode.Reliable, MessageID.HostCommand);
                message.AddString(userInput);
                ConnectionService.Client.Send(message);
                return;
            }

            switch (command)
            {
                case "requesthost":
                    {
                        if (PlayerHandler.Players[ConnectionService.Client.Id].IsHost)
                        {
                            ChatLog.Write("You already have host privileges");
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
                            ChatLog.Write(line);
                        }
                        break;
                    }
                case "msg":
                    {
                        if (args.Length == 0)
                        {
                            ChatLog.Write("Usage: /msg [message]");
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
                            ChatLog.Write("Usage: /whisper [client Id] [message]");
                            break;
                        }
                        if (!PlayerHandler.Players.ContainsKey(ushort.Parse(args[0])))
                        {
                            ChatLog.Write($"{args[0]} is not a valid client ID");
                            break;
                        }
                        string message = userInput[(userInput.IndexOf(' ') + 1 + args[0].Length + 1)..];
                        SendMessage(message, ushort.Parse(args[0]));
                        ChatLog.Write($"Sent message to client {ushort.Parse(args[0])}.");
                        break;
                    }
                default:
                    {
                        ChatLog.Write($"/{command} is not a command. Try /help for a list of commands");
                        break;
                    }
            }
        }

        private static void RequestHost()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ReqHost);
            ConnectionService.Client.Send(message);
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
            if (toClientId != null) message.AddUShort((ushort)toClientId);
            ConnectionService.Client.Send(message);
        }

        

        public void SetReady()
        {
            PlayerHandler.Players[ConnectionService.Client.Id].IsReady = !PlayerHandler.Players[ConnectionService.Client.Id].IsReady;
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
            message.AddBool(PlayerHandler.Players[ConnectionService.Client.Id].IsReady);
            ConnectionService.Client.Send(message);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    new Action(MainViewModel.Lobby.UpdateReadyStatus));
        }

        public void SetReady(ushort client)
        {
            PlayerHandler.Players[client].IsReady = !PlayerHandler.Players[client].IsReady;
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    new Action(MainViewModel.Lobby.UpdateReadyStatus));
        }

        
    }
}
