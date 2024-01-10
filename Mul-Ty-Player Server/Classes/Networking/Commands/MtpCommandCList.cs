using System;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandCList : Command
{
    public MtpCommandCList()
    {
        Name = "clist";
        Aliases = new List<string> { "clients", "cl" };
        Description = "List all clients connected to the server.";
        Usages = new List<string> { "/clist" };
        ArgDescriptions = new Dictionary<string, string>();
    }
    
    public override string InitExecute(string[] args)
    {
        return args.Length == 0 ? RunCList() : SuggestHelp();
    }
    
    public string RunCList()
    {
        string listRes = null;
        listRes += "\n--------------- Connected Clients ---------------\n";
        if (PlayerHandler.Players.Count == 0) listRes += "There are no clients currently connected.\n";
        foreach (var client in Server._Server.Clients.Where(x => x.IsConnected))
            listRes += "Client " + client.Id + " Name: " + PlayerHandler.Players[client.Id].Name + "\n";
        return listRes;
    }
}