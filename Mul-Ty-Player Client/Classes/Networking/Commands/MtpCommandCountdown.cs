using System;
using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandCountdown : Command
{
    public MtpCommandCountdown()
    {
        Name = "countdown";
        Aliases = new List<string> { "cd" };
        HostOnly = true;
        SpectatorAllowed = true;
        Usages = new List<string> { "/countdown start", "/countdown abort" };
        Description = "Start / stop the countdown. (Does not work in Hide & Seek Mode.)";
        ArgDescriptions = new Dictionary<string, string>
        {
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1 || 
            (!string.Equals(args[0], "start", StringComparison.CurrentCultureIgnoreCase) &&
             !string.Equals(args[0], "abort", StringComparison.CurrentCultureIgnoreCase)))
        {
            SuggestHelp();
            return;
        }
        RunCountdown(args[0]);
    }

    private void RunCountdown(string param)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Countdown);
        message.AddString(param);
        Client._client.Send(message);
    }
}