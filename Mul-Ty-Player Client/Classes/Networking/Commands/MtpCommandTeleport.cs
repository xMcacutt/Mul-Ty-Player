using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MulTyPlayer;
using MulTyPlayerClient.Classes.GamePlay;
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
        Usages = new List<string>
        {
            "/tp (teleports to last teleported POSITION)",
            "/tp <x> <y> <z>", "/tp <clientId>", "/tp <@1> <@2>", "/tp <@1> <x> <y> <z>", "/tp <@1> <posId>",
            "/tp <posId>"
        };
        Description = "Teleport to a specific location or other player in the same level.";
        ArgDescriptions = new Dictionary<string, string>
        {
            { "<x>", "x-coordinate to teleport to. Relative to current with ~x." },
            { "<y>", "x-coordinate to teleport to. Relative to current with ~y." },
            { "<z>", "z-coordinate to teleport to. Relative to current with ~z." },
            { "<clientId>", "client to teleport to." },
            { "<@1>", "Target selector from @a, @r, or clientId" },
            { "<@2>", "Target selector to @r, or clientId" },
            { "<posId>", "Level position identifier @s (start), @e (end)" }
        };
    }

    public override void InitExecute(string[] args)
    {
        args = args.Select(arg => arg.Replace(",", "")).ToArray();
        //CHECK ARGS
        if (args.Length > 4)
        {
            SuggestHelp();
            return;
        }

        //CHECK PRELIMS
        if (Client.HGameState.IsOnMainMenuOrLoading)
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
        if (args.Length == 0)
            RunTeleport();
    }

    private void RunTeleport()
    {
        Client.HHero.WriteHeldPosition();
    }

    private void RunTeleport(string selectorFrom, string x, string y, string z)
    {
        // bool true if value is clientId else false
        // ushort represents selector or clientId
        var from = ParseStringAsSelectorOrId(selectorFrom, SelectorType.From);
        if (from == null || 
            (!from.Value.Item1 && 
             ((Selector)from.Value.Item2 == Selector.LevelEnd || (Selector)from.Value.Item2 == Selector.LevelStart)))
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
        if (from == null || to == null || 
            (!from.Value.Item1 && 
             ((Selector)from.Value.Item2 == Selector.LevelEnd || (Selector)from.Value.Item2 == Selector.LevelStart)))
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
            var relCoord = 0f;
            if ((!float.TryParse(inCoord, out var absCoord) && !inCoord.StartsWith("~"))
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

    private void RunTeleport(string toString)
    {
        var to = ParseStringAsSelectorOrId(toString, SelectorType.To);
        if (to is null)
        {
            SuggestHelp();
            return;
        }
        if (!to.Value.Item1)
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
            message.AddBool(false);
            message.AddBool(true);
            message.AddUShort(Client._client.Id);
            message.AddBool(to.Value.Item1);
            message.AddUShort(to.Value.Item2);
            Client._client.Send(message);
            return;
        }
        
        if (!PlayerHandler.TryGetPlayer(to.Value.Item2, out var player))
        {
            LogError("The client id specified is not a player.");
            return;
        }
        if (to.Value.Item2 == Client._client.Id)
        {
            LogError("You can't teleport to yourself silly!");
            return;
        }
        var koalaId = Koalas.GetInfo[player.Koala].Id;
        var transform = PlayerReplication.PlayerTransforms[koalaId];
        if (transform.LevelID != Client.HLevel.CurrentLevelId)
        {
            Client.HLevel.ChangeLevel(transform.LevelID);
            var delayedTp = new Thread(() => DelayTP(3000, transform));
            delayedTp.Start();
            return;
        }
        Client.HHero.WritePosition(transform.Position.X, transform.Position.Y, transform.Position.Z);
    }

    private static void DelayTP(int millis, Transform transform)
    {
        Thread.Sleep(millis);
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
            case "s" or "S":
                selector = Selector.LevelStart;
                break;
            case "e" or "E":
                selector = Selector.LevelEnd;
                break;
            default:
                outSelector = null;
                return false;
        }

        var fromInValid = selectorType == SelectorType.From && selector is Selector.LevelEnd or Selector.LevelStart;
        var toInvalid = selectorType == SelectorType.To && selector is Selector.AllPlayers;
        if (fromInValid || toInvalid)
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
        if (message.UnreadLength == 2 || message.UnreadLength == 6)
        {
            var toClient = message.GetUShort();
            var level = message.GetInt();
            Logger.Write($"Teleporting to client {toClient}");
            if (!PlayerHandler.TryGetPlayer(toClient, out var toPlayer))
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
                Client.HLevel.ChangeLevel(level);
                var delayedTp = new Thread(() => DelayTP(3000, transform));
                delayedTp.Start();
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
    RandomPlayer,
    LevelStart,
    LevelEnd
}

public enum SelectorType : ushort
{
    From,
    To
}
