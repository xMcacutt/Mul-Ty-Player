using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using Octokit;
using Riptide;
using Message = Riptide.Message;

namespace MulTyPlayerClient;

public class MtpCommandAlert : Command
{
    public MtpCommandAlert()
    {
        Name = "alert";
        Aliases = new List<string> { "al" };
        HostOnly = true;
        SpectatorAllowed = true;
        Usages = new List<string> { "/alert <message>" };
        Description = "Uses your ability if present in Hide & Seek";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<message>", "Message to send with the alert."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length < 1)
        {
            SuggestHelp();
            return;
        }

        RunAlert(string.Join(' ', args));
    }

    public void RunAlert(string message)
    {
        var msg = Message.Create(MessageSendMode.Reliable, MessageID.Alert);
        msg.AddString(message);
        Client._client.Send(msg);
    }
}