using System.Collections.Generic;

namespace MulTyPlayerClient;

public class MtpCommandReady : Command
{
    public MtpCommandReady()
    {
        Name = "ready";
        Aliases = new List<string> { "r" };
        HostOnly = false;
        SpectatorAllowed = false;
        Usages = new List<string> { "/ready" };
        Description = "Set ready status.";
        ArgDescriptions = new Dictionary<string, string>();
    }

    public override void InitExecute(string[] args)
    {
        if (args.Length != 0)
        {
            SuggestHelp();
            return;
        }
        Client.HPlayer.SetReady();
    }
}