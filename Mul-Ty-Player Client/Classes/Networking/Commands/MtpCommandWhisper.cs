using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

public class MtpCommandWhisper : Command
{

    public MtpCommandWhisper()
    {
        Name = "whisper";
        Aliases = new List<string> { "w", "pm" };
        HostOnly = false;
        Usages = new List<string> { "/whisper <clientId> <message>" };
        Description = "Send a message to a specific client.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<clientId>", "Client to send the message to."},
            {"<message>", "Message to send to the client."},
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length < 2 || !ushort.TryParse(args[0], out var clientId))
        { 
            SuggestHelp();
            return;
        }
        if (PlayerHandler.Players.All(x => x.Id != clientId))
        {
            LogError($"{args[0]} is not a valid client ID");
            return;
        }
        RunWhisper(clientId, string.Join(' ', args.Skip(1)));
    }

    public void RunWhisper(ushort clientId, string message)
    {
        PeerMessageHandler.SendMessage(message, clientId);
        var name = PlayerHandler.TryGetPlayer(clientId, out var p) ? p.Name : "UnknownPlayer";
        Logger.Write($"Sent message to {name}.");
    }
}