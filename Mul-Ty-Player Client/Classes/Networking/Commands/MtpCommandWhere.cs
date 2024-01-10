﻿using System.Collections.Generic;
using System.Windows.Documents;
using MulTyPlayerClient.Classes.Networking;

namespace MulTyPlayerClient;

public class MtpCommandWhere : Command
{
    public MtpCommandWhere()
    {
        Name = "where";
        Aliases = new List<string> { "position", "pos" };
        HostOnly = false;
        Usages = new List<string> { "/where", "/where <clientId>" };
        Description = "Print your or a specified client's position.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<clientId>", "client to print position of."}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length > 1)
        {
            SuggestHelp();
            return;
        }
        if (Client.HGameState.IsAtMainMenuOrLoading())
        {
            LogError("Cannot get location on main menu or load screen.");
            return;
        }
        
        if (args.Length == 0)
            RunWhere();
        if (args.Length == 1)
            RunWhere(args[0]);
    }

    private void RunWhere()
    {
        var coords = Client.HHero.GetCurrentPosRot();
        Logger.Write($"You are at {coords[0]}, {coords[1]}, {coords[2]}");
    }
    
    private void RunWhere(string clientIdString)
    {
        if (!ushort.TryParse(clientIdString, out var clientId))
        {
            LogError("The id given is not valid");
            return;
        }
        if (!PlayerHandler.Players.ContainsKey(clientId))
        {
            LogError("The client id specified is not a player.");
            return;
        }
        if (clientId == Client._client.Id)
        {
            LogError("Use /where with no argument for your own location");
            return;
        }
        var player = PlayerHandler.Players[clientId];
        var koalaId = Koalas.GetInfo[player.Koala].Id;
        var transform = PlayerReplication.PlayerTransforms[koalaId];
        Logger.Write($"{player.Name} is in {Levels.GetLevelData(transform.LevelID).Code} at {transform.Position.X}, {transform.Position.Y}, {transform.Position.Z}");
    }
}