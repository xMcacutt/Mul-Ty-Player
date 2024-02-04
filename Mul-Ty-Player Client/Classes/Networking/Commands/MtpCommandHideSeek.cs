using System;
using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandHideSeek : Command
{
    public MtpCommandHideSeek()
    {
        Name = "hideseek";
        Aliases = new List<string> { "hs" };
        HostOnly = true;
        Usages = new List<string> { "/hideseek <true/false>", "/hideseek abort", "/hideseek range <x>" };
        Description = "Activate / Deactivate Hide & Seek mode or stop the current Hide & Seek session.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<true/false>", "Whether Hide & Seek mode should be turned on or off."},
            {"<x>", "Floating point range for hit detection."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            SuggestHelp();
            return;
        }
        if (args.Length is 2)
        {
            if (!string.Equals(args[0], "range", StringComparison.CurrentCultureIgnoreCase))
            {
                SuggestHelp();
                return;
            }
            if (!float.TryParse(args[1], out var range))
            {
                LogError("Invalid specified float");
                return;
            }
            RunHideSeek(range);
            return;
        }
        var doHideSeek = 
            string.Equals(args[0], "true", StringComparison.OrdinalIgnoreCase) ? true :
            string.Equals(args[0], "false", StringComparison.OrdinalIgnoreCase) ? false : 
            (bool?)null;
        var argIsAbort = string.Equals(args[0], "abort", StringComparison.CurrentCultureIgnoreCase);
        if (doHideSeek == null && !argIsAbort)
        {
            SuggestHelp();
            return;
        }
        if (argIsAbort)
            RunHideSeekAbort();
        else
            RunHideSeek((bool)doHideSeek);
    }

    private void RunHideSeekAbort()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_Abort);
        Client._client.Send(message);
    }

    private void RunHideSeek(bool value)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_ProxyRunHideSeek);
        if (value == false)
            RunHideSeekAbort();
        message.AddBool(value);
        Client._client.Send(message);
    }
    
    private void RunHideSeek(float range)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_ProxyRunHideSeek);
        message.AddFloat(range);
        Client._client.Send(message);
    }
}