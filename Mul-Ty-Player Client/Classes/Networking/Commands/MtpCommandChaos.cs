using System;
using System.Collections.Generic;

namespace MulTyPlayerClient;

public class MtpCommandChaos : Command
{
    public MtpCommandChaos()
    {
        Name = "chaos";
        Aliases = new List<string> { "ch" };
        HostOnly = true;
        SpectatorAllowed = true;
        Usages = new List<string> { "/chaos shuffle", "/chaos seed", "/chaos seed <new seed>" };
        Description = "Chaos Mode settings.";
        ArgDescriptions = new Dictionary<string, string>()
        {
            { "shuffle", "Sets a new seed, shuffling the positions of collectibles."},
            { "seed", "Prints the current seed."},
            { "<new seed>", "The new seed to change to."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length is not 1 and not 2)
        {
            SuggestHelp();
            return;
        }

        if (string.Equals(args[0], "seed", StringComparison.CurrentCultureIgnoreCase))
        {
            if (args.Length == 2)
            {
                if (int.TryParse(args[1], out var newSeed))
                {
                    ChaosHandler.RequestSeed(newSeed);
                    return;
                }

                SuggestHelp();
                return;
            }
            Logger.Write($"The current seed is: {Client.HChaos.ChaosSeed}");
            return;
        }
        else if (string.Equals(args[0], "shuffle", StringComparison.CurrentCultureIgnoreCase))
            ChaosHandler.RequestShuffle();
        else
        {
            SuggestHelp();
        }
    }
}