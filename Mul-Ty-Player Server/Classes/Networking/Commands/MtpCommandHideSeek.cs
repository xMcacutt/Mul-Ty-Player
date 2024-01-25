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
        Usages = new List<string> { "/hideseek <true/false>" };
        ArgDescriptions = new Dictionary<string, string>()
        {
            { "<true/false>", "Whether Hide & Seek mode should be turned on or off."}
        };
    }

    public override string InitExecute(string[] args)
    {
        if (args.Length != 1)
            return SuggestHelp();
        var doHideSeek = args[0] == "true" ? true : (args[0] == "false" ? false : (bool?)null);
        return doHideSeek == null ? SuggestHelp() : RunHideSeek((bool)doHideSeek);
    }
    
    private static string RunHideSeek(bool value)
    {
        SettingsHandler.DoHideSeek = value;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_SetHideSeekMode);
        message.AddBool(value);
        Server._Server.SendToAll(message);
        return $"Hide & Seek mode set to {value}";
    }

    [MessageHandler((ushort)MessageID.HS_SetHideSeekMode)]
    private static void ProxyRunHideSeek(ushort fromClientId, Message message)
    {
        Console.WriteLine(RunHideSeek(message.GetBool()));
    }
}