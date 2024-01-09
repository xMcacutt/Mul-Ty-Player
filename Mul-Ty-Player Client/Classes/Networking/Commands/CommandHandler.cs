using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class CommandHandler
{
    public Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);
    public Stack<string> Calls = new Stack<string>();
    public Stack<string> UndoCalls = new Stack<string>();

    public CommandHandler()
    {
        RegisterCommand(new MtpCommandHelp());
        RegisterCommand(new MtpCommandReady());
        RegisterCommand(new MtpCommandRequestHost());
        RegisterCommand(new MtpCommandTeleport());
        RegisterCommand(new MtpCommandWhere());
        RegisterCommand(new MtpCommandWhisper());
    }
    
    public void ParseCommand(string input)
    {
        if (input == null) return;
        Calls.Push(input);
        if (!input.StartsWith('/'))
        {
            PeerMessageHandler.SendMessage(input);
            return;
        }
        var commandName = input.Split(' ')[0].Trim('/');
        var args = input.Split(' ').Skip(1).ToArray();

        if (Commands.TryGetValue(commandName, out var command))
        {
            if (command.HostOnly)
            {
                if (PlayerHandler.Players[Client._client.Id].IsHost)
                {
                    var message = Message.Create(MessageSendMode.Reliable, MessageID.HostCommand);
                    message.AddString(input);
                    Client._client.Send(message);
                    return;
                }
                Logger.Write("[ERROR] You do not have the privileges to run this command.");
                return;
            }
            command.InitExecute(args);
            return;
        }
        Logger.Write("[Error] The command entered is not known.\nUse /clist for a list of commands.");
    }

    public void RegisterCommand(Command command)
    {
        Commands[command.Name] = command;
        foreach (var alias in command.Aliases)
        {
            Commands[alias] = command;
        }
    }
    
    [MessageHandler((ushort)MessageID.HostCommand)]
    public static void HostCommandResponse(Message message)
    {
        Logger.Write(message.GetString());
    }
}