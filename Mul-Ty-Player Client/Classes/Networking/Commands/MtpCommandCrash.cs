

using System.Collections.Generic;
using System.Diagnostics;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandCrash : Command
{
    public MtpCommandCrash()
    {
        Name = "crash";
        Aliases = new List<string> { "close", "c" };
        HostOnly = true;
        SpectatorAllowed = true;
        Usages = new List<string> { "/crash" };
        Description = "Attempts to crash every player's game, forcing a restart.";
        ArgDescriptions = new Dictionary<string, string>
        {
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length > 1)
        {
            SuggestHelp();
            return;
        }

        RunCrash();
    }

    private void RunCrash()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Crash);
        Client._client.Send(message);
        TyProcess.CloseProcess();
    }

    [MessageHandler((ushort)MessageID.Crash)]
    private static void CrashGame(Message message)
    {
        TyProcess.CloseProcess();
    }
}