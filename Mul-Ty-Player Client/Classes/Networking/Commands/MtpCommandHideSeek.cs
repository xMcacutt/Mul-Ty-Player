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
        Usages = new List<string> { "/hideseek <true/false>" };
        Description = "Activate / Deactivate Hide & Seek mode.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<true/false>", "Whether Hide & Seek mode should be turned on or off."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1)
        {
            SuggestHelp();
            return;
        }
        var doHideSeek = 
            string.Equals(args[0], "true", StringComparison.OrdinalIgnoreCase) ? true :
            string.Equals(args[0], "false", StringComparison.OrdinalIgnoreCase) ? false : 
            (bool?)null;
        if (doHideSeek == null)
        {
            SuggestHelp();
            return;
        }
        RunHideSeek((bool)doHideSeek);
    }

    private void RunHideSeek(bool value)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_SetHideSeekMode);
        message.AddBool(value);
        Client._client.Send(message);
    }
}