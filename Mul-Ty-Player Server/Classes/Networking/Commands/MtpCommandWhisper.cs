using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandWhisper : Command
{
    public MtpCommandWhisper()
    {
        Name = "whisper";
        Aliases = new List<string> { "w", "msg", "pm" };
        Usages = new List<string> { "/whisper <clientId> <message>" };
        Description = "Send a message to a specific client.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<clientId>", "Client to send the message to."},
            {"<message>", "Message to send to the client."},
        };
    }
    
    public override string InitExecute(string[] args)
    {
        if (args.Length < 2 || !ushort.TryParse(args[0], out var clientId))
            return SuggestHelp();
        return !PlayerHandler.Players.ContainsKey(clientId) ? 
            LogError($"{args[0]} is not a valid client ID") : 
            RunWhisper(clientId, string.Join(' ', args.Skip(1)));
    }

    public string RunWhisper(ushort clientId, string message)
    {
        PeerMessageHandler.SendMessageToClient(message, false, clientId);
        return $"Message sent to {PlayerHandler.Players[clientId].Name}.";
    }
}