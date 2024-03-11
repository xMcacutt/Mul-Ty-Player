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
        RegisterCommand(new MtpCommandCommandList());
        RegisterCommand(new MtpCommandCountdown());
        RegisterCommand(new MtpCommandHelp());
        RegisterCommand(new MtpCommandKick());
        RegisterCommand(new MtpCommandLevelLock());
        RegisterCommand(new MtpCommandPassword());
        RegisterCommand(new MtpCommandReady());
        RegisterCommand(new MtpCommandRequestHost());
        RegisterCommand(new MtpCommandResetSync());
        RegisterCommand(new MtpCommandTeleport());
        RegisterCommand(new MtpCommandWhere());
        RegisterCommand(new MtpCommandWhisper());
        RegisterCommand(new MtpCommandHideSeek());
        RegisterCommand(new MtpCommandGroundSwim());
        RegisterCommand(new MtpCommandCrash());
        RegisterCommand(new MtpCommandTaunt());
        RegisterCommand(new MtpCommandCheat());
        RegisterCommand(new MtpCommandLevel());
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
        var args = input.TrimEnd().Split(' ').Skip(1).Where(x => !string.IsNullOrEmpty(x)).ToArray();

        if (Commands.TryGetValue(commandName, out var command))
        {
            if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
            {
                Logger.Write("[ERROR] Failed to find self in player list.");
                return;
            }
            if (command.HostOnly && !self.IsHost)
            {
                Logger.Write("[ERROR] You do not have the privileges to run this command.");
                return;
            }
            command.InitExecute(args);
            return;
        }
        Logger.Write("[Error] The command entered is not known.\nUse /clist for a list of commands.");
    }

    private void RegisterCommand(Command command)
    {
        Commands[command.Name] = command;
        foreach (var alias in command.Aliases)
        {
            Commands[alias] = command;
        }
    }
}