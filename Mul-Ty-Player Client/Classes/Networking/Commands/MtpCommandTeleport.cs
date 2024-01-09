using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MulTyPlayerClient.Classes.Networking;

namespace MulTyPlayerClient;

public class MtpCommandTeleport : Command
{
    public MtpCommandTeleport()
    {
        Name = "tp";
        Aliases = new List<string> { "teleport", "tele" };
        HostOnly = false;
        Usages = new List<string> { "/tp <x> <y> <z>", "/tp <clientId>" };
        Description = "Teleport to a specific location or other player in the same level.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<x>", "x-coordinate to teleport to. Relative to current with ~x."},
            {"<y>", "x-coordinate to teleport to. Relative to current with ~y."},
            {"<z>", "z-coordinate to teleport to. Relative to current with ~z."},
            {"<clientId>", "client to teleport to."}
        };
    }

    public override void InitExecute(string[] args)
    {
        //CHECK ARGS
        if (args.Length is 0 or 2 or > 3)
        {
            SuggestHelp();
            return;
        }
        //CHECK PRELIMS
        if (Client.HGameState.IsAtMainMenuOrLoading())
        {
            LogError("Cannot teleport on main menu or load screen.");
            return;
        }
        //INFER AND RUN
        if (args.Length == 3)
            RunTeleport(args[0], args[1], args[2]);
        if (args.Length == 1)
            RunTeleport(args[0]);
    }

    private void RunTeleport(string x, string y, string z)
    {
        var inCoords = new string[] { x, y, z };
        var outCoords = new float[3];
        var currentPosRot = Client.HHero.GetCurrentPosRot();
        for (var i = 0; i < 3; i++)
        {
            var inCoord = inCoords[i];
            var absCoord = 0f;
            var relCoord = 0f;
            if ((!float.TryParse(inCoord, out absCoord) && !inCoord.StartsWith("~"))
                || (inCoord.StartsWith("~") && inCoord != "~" && !float.TryParse(inCoord.Skip(1).ToArray(), out relCoord)))
            {
                LogError("Coordinates specified are not valid");
                return;
            }
            if (!inCoord.StartsWith("~"))
            {
                outCoords[i] = absCoord;
                continue;
            }
            outCoords[i] = currentPosRot[i] + relCoord;
        }
        Client.HHero.WritePosition(outCoords[0], outCoords[1], outCoords[2]);
    }

    private void RunTeleport(string toClient)
    {
        if (!ushort.TryParse(toClient, out var clientId))
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
            LogError("You can't teleport to yourself silly!");
            return;
        }
        
        var player = PlayerHandler.Players[clientId];
        var koalaId = Koalas.GetInfo[player.Koala].Id;
        var transform = PlayerReplication.PlayerTransforms[koalaId];
        if (transform.LevelID != Client.HLevel.CurrentLevelId)
        {
            LogError("Cannot teleport to player in a different level");
            return;
        }
        Client.HHero.WritePosition(transform.Position.X, transform.Position.Y, transform.Position.Z);
    }
}