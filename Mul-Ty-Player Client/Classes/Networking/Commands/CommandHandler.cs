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
        RegisterCommand(new MtpCommandTeleport());
        RegisterCommand(new MtpCommandWhere());
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

        if (Commands.TryGetValue(commandName, out Command command))
        {
            if (command.HostOnly && !PlayerHandler.Players[Client._client.Id].IsHost)
            {
                Logger.Write("[ERROR] You do not have the privileges to run this command.");
                return;
            }
            command.InitExecute(args);
        }
    }

    public void RegisterCommand(Command command)
    {
        Commands[command.Name] = command;
        foreach (var alias in command.Aliases)
        {
            Commands[alias] = command;
        }
    }
    
}