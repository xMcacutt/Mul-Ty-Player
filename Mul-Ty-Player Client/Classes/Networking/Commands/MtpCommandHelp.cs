using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

public class MtpCommandHelp : Command
{
    public MtpCommandHelp()
    {
        Name = "help";
        Aliases = new List<string> { "h", "?" };
        HostOnly = false;
        SpectatorAllowed = true;
        Usages = new List<string> { "/help", "/help <command>" };
        Description = "Display command information";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<command>", "command to display info for."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length > 1)
        {
            SuggestHelp();
            return;
        }
        if (args.Length == 0)
            RunHelp();
        if (args.Length == 1)
            RunHelp(args[0]);
    }

    private void RunHelp()
    {
        foreach(var command in Client.HCommand.Commands.Where(x => x.Value.Name == x.Key))
            command.Value.PrintHelp();
    }

    private void RunHelp(string commandName)
    {
        if (!Client.HCommand.Commands.TryGetValue(commandName, out var command))
        {
            LogError("Command does not exist.");
            return;
        }
        command.PrintHelp();
    }
}