using System.Collections.Generic;
using System.Linq;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandHelp : Command
{
    public MtpCommandHelp()
    {
        Name = "help";
        Aliases = new List<string> { "h", "?" };
        Usages = new List<string> { "/help", "/help <command>" };
        Description = "Display command information";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<command>", "command to display info for."}
        };
    }
    
    public override string InitExecute(string[] args)
    {
        return args.Length switch
        {
            > 1 => SuggestHelp(),
            0 => RunHelp(),
            1 => RunHelp(args[0]),
            _ => null
        };
    }

    private string RunHelp()
    {
        foreach(var command in Program.HCommand.Commands.Where(x => x.Value.Name == x.Key))
            command.Value.PrintHelp();
        return null;
    }

    private string RunHelp(string commandName)
    {
        if (!Program.HCommand.Commands.TryGetValue(commandName, out var command))
            return LogError("Command does not exist.");
        command.PrintHelp();
        return null;
    }
}