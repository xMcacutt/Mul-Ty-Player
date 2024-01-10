using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandKick : Command
{
    public MtpCommandKick()
    {
        Name = "kick";
        Aliases = new List<string> { "k" };
        HostOnly = true;
        Usages = new List<string> { "/kick <clientId>" };
        Description = "Remove a client from the server.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<clientId>", "Client to kick."}
        };
    }

    public override void InitExecute(string[] args)
    {
        if (args.Length != 1)
        {
            SuggestHelp();
            return;
        }

        if (!ushort.TryParse(args[0], out var clientId))
        {
            LogError("Client id specified is not valid.");
            return;
        }

        if (clientId == Client._client.Id)
        {
            LogError("You cannot kick yourself.");
            return;
        }

        RunKick(clientId);
    }

    public void RunKick(ushort clientId)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.Kick);
        message.AddUShort(clientId);
        Client._client.Send(message);
    }
}