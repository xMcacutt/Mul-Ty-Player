using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandResetSync : Command
{
    public MtpCommandResetSync()
    {
        Name = "resetsync";
        Aliases = new List<string> { "reset", "rs" };
        Usages = new List<string> { "/resetsync" };
        Description = "Reset synchronisations to new game state.";
        ArgDescriptions = new Dictionary<string, string>();
    }
    
    public override string InitExecute(string[] args)
    {
        if (args.Length != 0)
            return SuggestHelp();
        RunResetSync();
        return "Synchronizations have been reset.";
    }

    private static void RunResetSync()
    {
        Program.HSync = new SyncHandler();
        SyncHandler.SendResetSyncMessage();
    }

    [MessageHandler((ushort)MessageID.ResetSync)]
    public static void ProxyResetSync(ushort fromClientId, Message message)
    {
        RunResetSync();
    }
}
