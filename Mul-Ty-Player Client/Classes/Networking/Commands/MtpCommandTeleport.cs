using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandTeleport : Command
{
    public MtpCommandTeleport()
    {
        Name = "tp";
        Aliases = new List<string> { "teleport", "tele" };
        HostOnly = false;
        Usages = new List<string> { "/tp <x> <y> <z>", "/tp <clientId>", "/tp <@1> <@2>", "/tp <@1> <x> <y> <z>" };
        Description = "Teleport to a specific location or other player in the same level.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<x>", "x-coordinate to teleport to. Relative to current with ~x."},
            {"<y>", "x-coordinate to teleport to. Relative to current with ~y."},
            {"<z>", "z-coordinate to teleport to. Relative to current with ~z."},
            {"<clientId>", "client to teleport to."},
            {"<@1>", "Target selector from @a, @r, or clientId"},
            {"<@2>", "Target selector to @r, or clientId"}
        };
    }

    public override void InitExecute(string[] args)
    {
        //CHECK ARGS
        if (args.Length is 0 or > 4)
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
        if (args.Length == 4)
            RunTeleport(args[0], args[1], args[2], args[3]);
        if (args.Length == 3)
            RunTeleport(args[0], args[1], args[2]);
        if (args.Length == 2)
            RunTeleport(args[0], args[1]);
        if (args.Length == 1)
            RunTeleport(args[0]);
    }

    private void RunTeleport(string selectorFrom, string x, string y, string z)
    {
        // bool true if value is clientId else false
        // ushort represents selector or clientId
        var from = ParseStringAsSelectorOrId(selectorFrom, SelectorType.From);
        if (from == null)
        {
            SuggestHelp();
            return;
        }
        var inCoords = new string[] { x, y, z };
        if (!TryParseCoords(inCoords, out var outCoords))
            return;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
        // Coordinates
        message.AddBool(true);
        message.AddBool(from.Value.Item1);
        message.AddUShort(from.Value.Item2);
        message.AddStrings(inCoords);
        Client._client.Send(message);
    }
    
    private void RunTeleport(string selectorFrom, string selectorTo)
    {
        // bool true if value is clientId else false
        // ushort represents selector or clientId
        var from = ParseStringAsSelectorOrId(selectorFrom, SelectorType.From);
        var to = ParseStringAsSelectorOrId(selectorTo, SelectorType.To);
        if (from == null || to == null)
        {
            SuggestHelp();
            return;
        }
        var message = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
        // Not coordinates
        message.AddBool(false);
        message.AddBool(from.Value.Item1);
        message.AddUShort(from.Value.Item2);
        message.AddBool(to.Value.Item1);
        message.AddUShort(to.Value.Item2);
        Client._client.Send(message);
    }

    private void RunTeleport(string x, string y, string z)
    {
        var inCoords = new string[] { x, y, z };
        if(TryParseCoords(inCoords, out var outCoords))
            Client.HHero.WritePosition(outCoords[0], outCoords[1], outCoords[2]);
    }

    private bool TryParseCoords(string[] inCoords, out float[] outCoords)
    {
        var coords = new float[3];
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
                outCoords = null;
                return false;
            }
            if (!inCoord.StartsWith("~"))
            {
                coords[i] = absCoord;
                continue;
            }
            coords[i] = currentPosRot[i] + relCoord;
        }
        outCoords = coords;
        return true;
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

    private (bool, ushort)? ParseStringAsSelectorOrId(string selector, SelectorType type)
    {
        if (selector.StartsWith('@') && TryParseSelector(selector, type, out var parsedSelector))
            return (false, (ushort)parsedSelector);
        if (ushort.TryParse(selector, out var parsedId))
            return (true, parsedId);
        return null;
    }

    private bool TryParseSelector(string selectorChar, SelectorType selectorType, out Selector? outSelector)
    {
        selectorChar = selectorChar[1..];
        Selector? selector;
        switch (selectorChar)
        {
            case "a" or "A":
                selector = Selector.AllPlayers;
                break;
            case "r" or "R":
                selector = Selector.RandomPlayer;
                break;
            default:
                outSelector = null;
                return false;
        }
        if (selectorType == SelectorType.To && selector == Selector.AllPlayers)
        {
            outSelector = null;
            return false;
        }
        outSelector = selector;
        return true;
    }

    [MessageHandler((ushort)MessageID.AdvancedTeleport)]
    private static void ProxyTeleport(Message message)
    {
        if (message.UnreadLength == 2)
        {
            var toClient = message.GetUShort();
            Logger.Write($"Teleporting to client {toClient}");
            if (!PlayerHandler.Players.TryGetValue(toClient, out var toPlayer))
            {
                Logger.Write("[ERROR] Could not find player.");
                return;
            }
            if (Client._client.Id == toPlayer.Id)
            {
                Logger.Write("[ERROR] Attempted to teleport to self. Aborting.");
                return;
            }
            var koalaId = Koalas.GetInfo[toPlayer.Koala].Id;
            if (!PlayerReplication.PlayerTransforms.TryGetValue(koalaId, out var transform) || transform.LevelID != Client.HLevel.CurrentLevelId)
            {
                Logger.Write("[ERROR] Cannot teleport to player in a different level");
                return;
            }
            Client.HHero.WritePosition(transform.Position.X, transform.Position.Y, transform.Position.Z);
        }
        else
        {
            var coordinates = message.GetFloats();
            Client.HHero.WritePosition(coordinates[0], coordinates[1], coordinates[2]);
        }
    }
}

public enum Selector : ushort
{
    AllPlayers,
    RandomPlayer
}

public enum SelectorType : ushort
{
    From,
    To
}
