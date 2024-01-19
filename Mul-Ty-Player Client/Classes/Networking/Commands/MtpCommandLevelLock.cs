using System;
using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandLevelLock : Command
{
    public MtpCommandLevelLock()
    {
        Name = "levellock";
        Aliases = new List<string> { "ll" };
        HostOnly = true;
        Usages = new List<string> { "/levellock <true/false>" };
        Description = "Activate / Deactivate Level Lock mode.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<true/false>", "Whether Level Lock mode should be turned on or off."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1)
        {
            SuggestHelp();
            return;
        }
        var doLevelLock = 
            string.Equals(args[0], "true", StringComparison.OrdinalIgnoreCase) ? true :
            string.Equals(args[0], "false", StringComparison.OrdinalIgnoreCase) ? false : 
            (bool?)null;
        if (doLevelLock == null)
        {
            SuggestHelp();
            return;
        }
        RunLevelLock((bool)doLevelLock);
    }

    private void RunLevelLock(bool value)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SetLevelLock);
        message.AddBool(value);
        Client._client.Send(message);
    }
}