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
        Usages = new List<string> { "/hideseek <true/false>", "/hideseek range <x>" };
        ArgDescriptions = new Dictionary<string, string>()
        {
            { "<true/false>", "Whether Hide & Seek mode should be turned on or off."},
            { "<x>", "Floating point range for hit detection."}
        };
    }

    public override string InitExecute(string[] args)
    {
        if (args.Length is not 1 or 2)
            return SuggestHelp();
        if (args.Length is 2)
        {
            if (!string.Equals(args[1], "range", StringComparison.CurrentCultureIgnoreCase))
                return SuggestHelp();
            if (float.TryParse(args[2], out var range))
                return LogError("Invalid specified float");
            return RunHideSeek(range);
        }
        var doHideSeek = args[0] == "true" ? true : (args[0] == "false" ? false : (bool?)null);
        return doHideSeek == null ? SuggestHelp() : RunHideSeek((bool)doHideSeek);
    }

    private static string RunHideSeek(float range)
    {
        SettingsHandler.HSRange = range;
        return $"Range set to {range}";
    }
    
    private static string RunHideSeek(bool value)
    {
        SettingsHandler.DoHideSeek = value;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_ProxyRunHideSeek);
        message.AddBool(value);
        Server._Server.SendToAll(message);
        return $"Hide & Seek mode set to {value}";
    }

    [MessageHandler((ushort)MessageID.HS_ProxyRunHideSeek)]
    private static void ProxyRunHideSeek(ushort fromClientId, Message message)
    {
        if (message.UnreadLength > 1)
            Console.WriteLine(RunHideSeek(message.GetFloat()));
        else
            Console.WriteLine(RunHideSeek(message.GetBool()));
    }
}