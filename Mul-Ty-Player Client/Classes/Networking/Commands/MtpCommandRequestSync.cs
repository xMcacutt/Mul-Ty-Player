using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualBasic.Logging;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandRequestSync : Command
{
    public MtpCommandRequestSync()
    {
        Name = "requestsync";
        Aliases = new List<string> { "reqsync", "rqs" };
        HostOnly = false;
        SpectatorAllowed = true;
        Usages = new List<string> { "/requestsync" };
        Description = "Attempts to synchronise client and server.";
        ArgDescriptions = new Dictionary<string, string>();
    }

    public override void InitExecute(string[] args)
    {
        if (args.Length != 0)
        {
            SuggestHelp();
            return;
        }
        Client.HSync.RequestSync();
    }
}