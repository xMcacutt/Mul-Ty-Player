using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandResetSync : Command
{
    public MtpCommandResetSync()
    {
        Name = "resetsync";
        Aliases = new List<string> { "reset", "rs" };
        HostOnly = true;
        Usages = new List<string> { "/resetsync" };
        Description = "Reset collectible synchronisations to new game state.";
        ArgDescriptions = new Dictionary<string, string>();
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 0)
        {
            SuggestHelp();
            return;
        }
        RunResetSync();
    }

    private void RunResetSync()
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageID.ResetSync);
        Client._client.Send(message);
    }
}