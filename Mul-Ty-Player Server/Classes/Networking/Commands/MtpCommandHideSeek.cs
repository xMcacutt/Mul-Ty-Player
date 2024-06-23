using System;
using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandHideSeek : Command
{

    public MtpCommandHideSeek()
    {
        Name = "hideseek";
        Aliases = new List<string> { "hs" };
        Description = "Activate / Deactivate Hide & Seek mode.";
        Usages = new List<string> { "/hideseek range <x>" };
        ArgDescriptions = new Dictionary<string, string>()
        {
            { "<x>", "Floating point range for hit detection."}
        };
    }

    public override string InitExecute(string[] args)
    {
        if (args.Length is not 2)
            return SuggestHelp();
        else
        {
            if (!string.Equals(args[1], "range", StringComparison.CurrentCultureIgnoreCase))
                return SuggestHelp();
            if (float.TryParse(args[2], out var range))
                return LogError("Invalid specified float");
            return RunHideSeek(range);
        }
    }

    private static string RunHideSeek(float range)
    {
        SettingsHandler.HSRange = range;
        return $"Range set to {range}";
    }
}