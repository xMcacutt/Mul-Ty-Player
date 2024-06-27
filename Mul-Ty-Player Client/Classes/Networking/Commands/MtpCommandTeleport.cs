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
    public static Dictionary<int, float[]> LevelStarts = new Dictionary<int, float[]>()
    {
         {0, new float[] {71f, 2622.9421f, 209f} },
         {1, new float[] {0, 0, 0} },
         {2, new float[] {0, 0, 0} },
         {3, new float[] {0, 0, 0} },
         {4, new float[] {-3738.3555f, 351.45612f, 7932.4614f} },
         {5, new float[] {-8940f, -1453.5535f, 7162f} },
         {6, new float[] {-13646f, 338f, 22715f} },
         {7, new float[] {-572f, -493.60754f, -59f} },
         {8, new float[] {-3242f, -610.2542f, 6197f} },
         {9, new float[] {-467.61282f, -2624.3982f, 191.87332f} },
        {10, new float[] {-14216.542f, 4800.319f, 16626.096f} },
        {11, new float[] {0, 0, 0} },
        {12, new float[] {-4246f, -67.183754f, 1343f} },
        {13, new float[] {-5499f, -507.01822f, -6951f} },
        {14, new float[] {-5771f, -1689.4564f, -1658f} },
        {15, new float[] {2241f, -477.76157f, -568f} },
        {16, new float[] {0, 0, 0} },
        {17, new float[] {-6306f, -860.1768f, -7322f} },
        {18, new float[] {0, 0, 0} },
        {19, new float[] {-7861f, -508.86023f, 434f} },
        {20, new float[] {-8845f, 1700.0707f, 17487f} },
        {21, new float[] {-82f, 724.13086f, 449f} },
        {22, new float[] {-82f, 724.13086f, 449f} }, 
        {23, new float[] {12f, 0.05010605f, -2049f} },
    };
    public static Dictionary<int, float[]> LevelEnds = new Dictionary<int, float[]>()
    {
         {0, new float[] {71f, 2622.9421f, 209f} },
         {1, new float[] {0, 0, 0} },
         {2, new float[] {0, 0, 0} },
         {3, new float[] {0, 0, 0} },
         {4, new float[] {-8327.423f, 431.42093f, -1255.9796f} },
         {5, new float[] {-6062.538f, 247.70023f, 7424.3096f} },
         {6, new float[] {1576.1663f, 5765.201f, -11264.105f} },
         {7, new float[] {-572f, -493.60754f, -59f} },
         {8, new float[] {-7023.561f, 192.2511f, -7245.863f} },
         {9, new float[] {40595.652f, -383.56946f, 3621.8237f} },
        {10, new float[] {36711.64f, 3709.1973f, -18600.393f} },
        {11, new float[] {0, 0, 0} },
        {12, new float[] {-1232.5217f, -1501.8242f, 1479.3169f} },
        {13, new float[] {-9035.317f, 10568.717f, 17120.656f} },
        {14, new float[] {-13466.467f, -1694.2596f, -9534.123f} },
        {15, new float[] {2241f, -477.76157f, -568f} },
        {16, new float[] {0, 0, 0} },
        {17, new float[] {6748.3237f, 2641.4827f, -4747.0815f} },
        {18, new float[] {0, 0, 0} },
        {19, new float[] {-7861f, -508.86023f, 434f} },
        {20, new float[] {-9900.573f, -1471.7053f, -5911.128f} },
        {21, new float[] {-82f, 724.13086f, 449f} },
        {22, new float[] {-82f, 724.13086f, 449f} }, 
        {23, new float[] {-18.875942f, -1069.9381f, -1829.7476f} },
    };
    
    public MtpCommandTeleport()
    {
        Name = "tp";
        Aliases = new List<string> { "teleport", "tele" };
        HostOnly = false;
        SpectatorAllowed = false;
        Usages = new List<string>
        {
            "/tp (teleports to last teleported POSITION)",
            "/tp <x> <y> <z>", "/tp <clientId>", "/tp <@1> <@2>", "/tp <@1> <x> <y> <z>", "/tp <@1> <posId>",
            "/tp <posId>"
        };
        Description = "Teleport to a location based on the provided identifiers.";
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
        var koalaId = Koalas.GetInfo[(Koala)player.Koala].Id;
        var transform = PlayerReplication.PlayerTransforms[koalaId];
        if (transform.LevelId != Client.HLevel.CurrentLevelId)
        {
            Client.HLevel.ChangeLevel(transform.LevelId);
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
        if (message.UnreadBits < 96)
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
            var koalaId = Koalas.GetInfo[(Koala)toPlayer.Koala].Id;
            if (!PlayerReplication.PlayerTransforms.TryGetValue(koalaId, out var transform) || transform.LevelId != Client.HLevel.CurrentLevelId)
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
