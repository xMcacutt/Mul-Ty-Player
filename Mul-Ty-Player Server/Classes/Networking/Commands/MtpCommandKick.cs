using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandKick : Command
{
    public MtpCommandKick()
    {
        Name = "kick";
        Aliases = new List<string> { "k" };
        Usages = new List<string> { "/kick <clientId>" };
        Description = "Remove a client from the server.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<clientId>", "Client to kick."}
        };
    }
    
    public override string InitExecute(string[] args)
    {
        if (args.Length == 0) 
            return SuggestHelp();
        if (!ushort.TryParse(args[0], out var clientId) || !PlayerHandler.Players.ContainsKey(clientId))
            return LogError(clientId + " is not a valid client ID");
        RunKick(clientId);
        return $"Successfully Kicked {PlayerHandler.Players[clientId].Name}";
    }
    
    private static void RunKick(ushort clientId)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Disconnect);
        Server._Server.Send(message, clientId);
    }

    [MessageHandler((ushort)MessageID.Kick)]
    private static void ProxyRunKick(ushort fromClientId, Message message)
    {
        RunKick(message.GetUShort());
    }
}