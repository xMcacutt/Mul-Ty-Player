﻿using System;
using System.Collections.Generic;
using System.Linq;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class CommandHandler
{
    public Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);

    public CommandHandler()
    {
        RegisterCommand(new MtpCommandCList());
        RegisterCommand(new MtpCommandHelp());
        RegisterCommand(new MtpCommandKick());
        RegisterCommand(new MtpCommandLevelLock());
        RegisterCommand(new MtpCommandPassword());
        RegisterCommand(new MtpCommandResetSync());
        RegisterCommand(new MtpCommandRestart());
        RegisterCommand(new MtpCommandWhisper());
    }
    
    public string ParseCommand(string input)
    {
        if (input == null) return null;
        if (!input.StartsWith('/'))
        {
            PeerMessageHandler.SendMessageToClients(input, false);
            return "Sent message to all connected clients.";
        }
        var commandName = input.Split(' ')[0].Trim('/');
        var args = input.TrimEnd().Split(' ').Skip(1).Where(x => !string.IsNullOrEmpty(x)).ToArray();

        if (Commands.TryGetValue(commandName, out var command))
        {
            return command.InitExecute(args);
        }
        return "[Error] The command entered is not known.\nUse /help for a list of commands.";
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