using System.Collections.Generic;
using MulTyPlayer;
using MulTyPlayerServer.Sync.Objective;
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
        Program.HObjective = new ObjectiveHandler();
        HardcoreHandler.HardcoreRunDead = false; 
        SyncHandler.SendResetSyncMessage();
    }

    [MessageHandler((ushort)MessageID.ResetSync)]
    public static void ProxyResetSync(ushort fromClientId, Message message)
    {
        RunResetSync();
    }
}
