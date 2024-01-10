using System.Collections.Generic;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandRestart : Command
{
    public MtpCommandRestart()
    {
        Name = "restart";
        Aliases = new List<string> { "shutdown", "end" };
        Usages = new List<string> { "/restart" };
        Description = "Restart server.";
        ArgDescriptions = new Dictionary<string, string>();
    }
    
    public override string InitExecute(string[] args)
    {
        if (args.Length != 0)
            return SuggestHelp();
        return RunRestart();
    }

    public string RunRestart()
    {
        Server.RestartServer();
        return null;
    }
}