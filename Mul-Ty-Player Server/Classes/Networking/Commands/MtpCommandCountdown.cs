using System;
using System.Collections.Generic;
using System.Threading;
using MulTyPlayer;
using MulTyPlayerServer.Sync.Objective;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandCountdown : Command
{
    public MtpCommandCountdown()
    {
        Name = "countdown";
        Aliases = new List<string> { "cd" };
        Usages = new List<string> { "/countdown start", "/countdown abort" };
        Description = "Start / stop the countdown. (Does not work in Hide & Seek Mode.)";
        ArgDescriptions = new Dictionary<string, string>
        {
        };
    }
    
    public override string InitExecute(string[] args)
    {
        if (args.Length != 1 ||
            (!string.Equals(args[0], "start", StringComparison.CurrentCultureIgnoreCase) &&
             !string.Equals(args[0], "abort", StringComparison.CurrentCultureIgnoreCase)))
           return SuggestHelp();
        return RunCountdown(args[0]);
    }

    private static string RunCountdown(string param)
    {
        foreach (var entry in PlayerHandler.Players) 
            entry.Value.IsReady = false;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Countdown);
        message.AddString(param);
        Server._Server.SendToAll(message);
        return $"Countdown {param} message send to clients.";
    }
    
    [MessageHandler((ushort)MessageID.Countdown)]
    private static void HandleProxyCountdown(ushort fromClientId, Message message)
    {
        Program.HSync = new SyncHandler();
        Program.HObjective = new ObjectiveHandler();
        var param = message.GetString();
        Console.WriteLine(RunCountdown(param));
    }
    
    [MessageHandler((ushort)MessageID.CountdownFinishing)]
    private static void HandleCountdownFinishing(ushort fromClientId, Message message)
    {
        Program.HSync = new SyncHandler();
        Program.HObjective = new ObjectiveHandler();
    }
}