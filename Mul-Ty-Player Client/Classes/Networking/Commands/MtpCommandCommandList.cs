using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

public class MtpCommandCommandList : Command
{
    public MtpCommandCommandList()
    {
        Name = "clist";
        Aliases = new List<string> { "commands", "cmds" };
        HostOnly = false;
        SpectatorAllowed = true;
        Usages = new List<string> { "/clist" };
        Description = "Prints all available command names.";
        ArgDescriptions = new Dictionary<string, string>();
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 0)
        {
            SuggestHelp();
            return;
        }
        RunClist();
    }

    public void RunClist()
    {
        foreach (var command in Client.HCommand.Commands.Where(x => x.Value.Name == x.Key))
        {
            var cmdStr = command.Key;
            if (command.Value.HostOnly) cmdStr += " [HOST]";
            if (command.Value.SpectatorAllowed) cmdStr += " [NO SPECT]";
            Logger.Write(cmdStr);
        }
    }
}