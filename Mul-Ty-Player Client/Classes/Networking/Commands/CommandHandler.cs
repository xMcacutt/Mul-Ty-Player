using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class CommandHandler
{
    public Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);

    public CommandHandler()
    {
        RegisterCommand(new MtpCommandHelp());
        RegisterCommand(new MtpCommandTeleport());
    }
    
    public void ParseCommand(string input)
    {
        if (input == null) return;

        var commandName = input.Split(' ')[0].Trim('/');
        var args = input.Split(' ').Skip(1).ToArray();
        
        if (Commands.TryGetValue(commandName, out Command command))
            command.InitExecute(args);
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