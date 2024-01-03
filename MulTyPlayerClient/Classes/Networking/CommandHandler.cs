using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

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
            "levellock"
        };
    }

    public void ParseCommand(string userInput)
    {
        if (userInput == null) return;

        var command = userInput.Split(' ')[0].Trim('/');
        var args = userInput.Split(' ').Skip(1).ToArray();

        if (PlayerHandler.Players[Client._client.Id].IsHost && hostCommands.Contains(command))
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.HostCommand);
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
                    Logger.Write("You already have host privileges");
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
                foreach (var line in File.ReadLines("./help.mtps")) Logger.Write(line);
                break;
            }
            case "msg":
            {
                if (args.Length == 0)
                {
                    Logger.Write("Usage: /msg [message]");
                    break;
                }

                var message = userInput[5..];
                SendMessage(message, null);
                break;
            }
            case "whisper":
            {
                if (args.Length < 2 || !ushort.TryParse(args[0], out _))
                {
                    Logger.Write("Usage: /whisper [client Id] [message]");
                    break;
                }

                if (!PlayerHandler.Players.ContainsKey(ushort.Parse(args[0])))
                {
                    Logger.Write($"{args[0]} is not a valid client ID");
                    break;
                }

                var message = userInput[(userInput.IndexOf(' ') + 1 + args[0].Length + 1)..];
                SendMessage(message, ushort.Parse(args[0]));
                Logger.Write($"Sent message to client {ushort.Parse(args[0])}.");
                break;
            }
            default:
            {
                Logger.Write($"/{command} is not a command. Try /help for a list of commands");
                break;
            }
        }
    }

    private static void RequestHost()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqHost);
        Client._client.Send(message);
    }

    public static bool HostExists()
    {
        return PlayerHandler.Players.Values.Any(p => p.IsHost);
    }

    private static void SendMessage(string text, ushort? toClientId)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.P2PMessage);
        message.AddBool(toClientId == null);
        message.AddString(text);
        if (toClientId != null) message.AddUShort((ushort)toClientId);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.Ready)]
    public static void PeerReady(Message message)
    {
        PlayerHandler.Players[message.GetUShort()].IsReady = message.GetBool();
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.UpdateReadyStatus));
    }


    [MessageHandler((ushort)MessageID.SetLevelLock)]
    public static void SetLevelLock(Message message)
    {
        SettingsHandler.DoLevelLock = message.GetBool();
        var result = SettingsHandler.DoLevelLock ? "Level lock has been activated" : "Level lock has been disabled";
        Logger.Write(result);
    }

    public void SetReady()
    {
        PlayerHandler.Players[Client._client.Id].IsReady = !PlayerHandler.Players[Client._client.Id].IsReady;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Ready);
        message.AddBool(PlayerHandler.Players[Client._client.Id].IsReady);
        Client._client.Send(message);
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.UpdateReadyStatus));
    }

    [MessageHandler((ushort)MessageID.ReqHost)]
    public static void RequestHostResponse(Message message)
    {
        if (message.GetBool())
        {
            PlayerHandler.Players[Client._client.Id].IsHost = true;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(ModelController.Lobby.UpdateHostIcon));

            Logger.Write("You have been made host. You now have access to host only commands.");
            return;
        }

        try
        {
            var existingHost = PlayerHandler.Players.Values.First(p => p.IsHost);
            Logger.Write($"Player {existingHost.Name} already has host privileges");
        }
        catch
        {
            Logger.Write(
                "Existing host was found by server, but not by client. Possible error occured. Denying host privileges.");
        }
    }

    [MessageHandler((ushort)MessageID.HostChange)]
    public static void HostChange(Message message)
    {
        foreach (var key in PlayerHandler.Players.Keys) PlayerHandler.Players[key].IsHost = false;
        PlayerHandler.Players[message.GetUShort()].IsHost = true;
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.UpdateHostIcon));
    }

    [MessageHandler((ushort)MessageID.HostCommand)]
    public static void HostCommandResponse(Message message)
    {
        Logger.Write(message.GetString());
    }

    [MessageHandler((ushort)MessageID.ResetSync)]
    private static void HandleSyncReset(Message message)
    {
        Logger.Write("Synchronisations have been reset to new game state.");
        Client.HSync = new SyncHandler();
    }

    [MessageHandler((ushort)MessageID.P2PMessage)]
    private static void HandleMessageFromPeer(Message message)
    {
        Logger.Write(message.GetString());
    }
}