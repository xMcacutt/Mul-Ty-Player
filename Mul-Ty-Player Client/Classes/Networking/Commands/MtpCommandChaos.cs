using System;
using System.Collections.Generic;

namespace MulTyPlayerClient;

public class MtpCommandChaos : Command
{
    public MtpCommandChaos()
    {
        Name = "chaos";
        Aliases = new List<string> { "cm" };
        HostOnly = true;
        SpectatorAllowed = true;
        Usages = new List<string> { "/chaos shuffle", "/chaos seed" };
        Description = "Chaos Mode settings.";
        ArgDescriptions = new Dictionary<string, string>()
        {
            { "shuffle", "Sets a new seed, shuffling the positions of collectibles."},
            { "seed", "Prints the current seed."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1)
        {
            SuggestHelp();
            return;
        }
        if (string.Equals(args[0], "shuffle", StringComparison.CurrentCultureIgnoreCase))
            ChaosHandler.RequestShuffle();
        else if (string.Equals(args[0], "seed", StringComparison.CurrentCultureIgnoreCase))
            Logger.Write($"The current seed is: {Client.HChaos.ChaosSeed}");
        else
        {
            SuggestHelp();
        }
    }
}