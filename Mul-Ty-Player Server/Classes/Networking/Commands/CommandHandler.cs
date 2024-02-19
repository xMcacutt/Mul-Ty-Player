using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class CommandHandler
{
    public Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);

    public CommandHandler()
    {
        RegisterCommand(new MtpCommandCList());
        RegisterCommand(new MtpCommandCountdown());
        RegisterCommand(new MtpCommandHelp());
        RegisterCommand(new MtpCommandKick());
        RegisterCommand(new MtpCommandLevelLock());
        RegisterCommand(new MtpCommandPassword());
        RegisterCommand(new MtpCommandResetSync());
        RegisterCommand(new MtpCommandRestart());
        RegisterCommand(new MtpCommandWhisper());
        RegisterCommand(new MtpCommandHideSeek());
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

    [MessageHandler((ushort)MessageID.Crash)]
    private static void CrashClients(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.Crash);
        Server._Server.SendToAll(response, fromClientId);
    }
}