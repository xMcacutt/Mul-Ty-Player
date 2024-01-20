using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandTeleport
{

    private static Dictionary<int, float[]> _levelStarts = new Dictionary<int, float[]>()
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
        {15, new float[] {0, 0, 0} },
        {16, new float[] {0, 0, 0} },
        {17, new float[] {0, 0, 0} },
        {18, new float[] {0, 0, 0} },
        {19, new float[] {-7861f, -508.86023f, 434f} },
        {20, new float[] {0, 0, 0} },
        {21, new float[] {0, 0, 0} },
        {22, new float[] {0, 0, 0} }, 
        {23, new float[] {0, 0, 0} },
    };
    private static Dictionary<int, float[]> _levelEnds = new Dictionary<int, float[]>()
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
        {15, new float[] {0, 0, 0} },
        {16, new float[] {0, 0, 0} },
        {17, new float[] {0, 0, 0} },
        {18, new float[] {0, 0, 0} },
        {19, new float[] {-7861f, -508.86023f, 434f} },
        {20, new float[] {0, 0, 0} },
        {21, new float[] {0, 0, 0} },
        {22, new float[] {0, 0, 0} }, 
        {23, new float[] {0, 0, 0} },
    };
    
    private static void Teleport(bool fromIsClientId, ushort from, bool toIsClientId, ushort to, ushort sender)
    {
        var fromPlayer = GetPlayer(fromIsClientId, from, SelectorType.From);
        if (fromPlayer is null)
        {
            PeerMessageHandler.SendMessageToClient("[ERROR] Teleport failed, could not find player", false, sender);
            return;
        }

        if (!toIsClientId && ((Selector)to == Selector.LevelStart || (Selector)to == Selector.LevelEnd))
        {
            if (!fromIsClientId && (Selector)from == Selector.AllPlayers)
            {
                foreach (var player in PlayerHandler.Players.Values)
                {
                    
                }
            }
        }
        
        var toPlayer = GetPlayer(toIsClientId, to, SelectorType.To, fromPlayer);
        if (toPlayer is null)
        {
            PeerMessageHandler.SendMessageToClient("[ERROR] Teleport failed, could not find player", false, sender);
            return;
        }
        if (!fromIsClientId && (Selector)from == Selector.AllPlayers)
        {
            foreach (var player in PlayerHandler.Players.Values.Where(x => x.CurrentLevel == toPlayer.CurrentLevel))
            {
                if (player.ClientID == toPlayer.ClientID)
                    continue;
                var allMessage = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
                allMessage.AddUShort(toPlayer.ClientID);
                Server._Server.Send(allMessage, player.ClientID);
            }
            return;
        }

        if (!fromIsClientId && (Selector)from == Selector.RandomPlayer)
        {
            fromPlayer = GetRandomPlayer(SelectorType.To, toPlayer.CurrentLevel, toPlayer.ClientID);
            if (fromPlayer is null)
            {
                PeerMessageHandler.SendMessageToClient("[ERROR] Teleport failed, could not find player", false, sender);
                return;
            }
        }
        if (fromPlayer.ClientID == toPlayer.ClientID)
        {
            PeerMessageHandler.SendMessageToClient("[ERROR] Teleport failed, critical error.", false, sender);
            return;
        }

        if (fromPlayer.CurrentLevel != toPlayer.CurrentLevel)
        {
            PeerMessageHandler.SendMessageToClient("[ERROR] Teleport failed, players in different levels.", false, sender);
            return;
        }
        var message = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
        message.AddUShort(toPlayer.ClientID);
        Server._Server.Send(message, fromPlayer.ClientID);
    }

    private static void Teleport(bool fromIsClientId, ushort from, string[] coords, ushort sender)
    {
        var fromPlayer = GetPlayer(fromIsClientId, from, SelectorType.From);
        if (fromPlayer is null)
        {
            PeerMessageHandler.SendMessageToClient("[ERROR] Teleport failed, could not find player", false, sender);
            return;
        }
        if (!fromIsClientId && (Selector)from == Selector.AllPlayers)
        {
            foreach (var player in PlayerHandler.Players.Values)
            {
                if (!TryParseCoords(player.ClientID, coords, out var outCoordsPerPlayer, sender)) continue;
                var allMessage = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
                allMessage.AddFloats(outCoordsPerPlayer);
                Server._Server.Send(allMessage, player.ClientID);
            }
            return;
        }
        if (!TryParseCoords(fromPlayer.ClientID, coords, out var outCoords, sender))
            return;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
        message.AddFloats(outCoords);
        Server._Server.Send(message, fromPlayer.ClientID);
    }

    private static bool TryParseCoords(ushort clientId, string[] inCoords, out float[] outCoords, ushort sender)
    {
        if (!PlayerHandler.Players.TryGetValue(clientId, out var player))
        {
            PeerMessageHandler.SendMessageToClient("[ERROR] Player not found in server.", false, sender);
            outCoords = null;
            return false;
        }
        var coords = new float[3];
        var currentPosRot = player.Coordinates;
        for (var i = 0; i < 3; i++)
        {
            var inCoord = inCoords[i];
            var absCoord = 0f;
            var relCoord = 0f;
            if ((!float.TryParse(inCoord, out absCoord) && !inCoord.StartsWith("~"))
                || (inCoord.StartsWith("~") && inCoord != "~" && !float.TryParse(inCoord.Skip(1).ToArray(), out relCoord)))
            {
                PeerMessageHandler.SendMessageToClient("[ERROR] Coordinates specified are not valid", false, sender);
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
    
    private static Player GetPlayer(bool isClientId, ushort playerId, SelectorType selectorType, Player fromPlayer = null)
    {
        Player player = null;

        if (isClientId)
        {
            if (!PlayerHandler.Players.TryGetValue(playerId, out player))
            {
                return null;
            }
        }
        else
        {
            player = GetRandomPlayer(selectorType, fromPlayer?.CurrentLevel, fromPlayer?.ClientID);
            if (player is null)
            {
                return null;
            }
        }
        return player;
    }
    
    private static Player GetRandomPlayer(SelectorType selectorType, int? level, ushort? excludePlayerId)
    {
        var playerList = selectorType == SelectorType.To
            ? PlayerHandler.Players.Values.Where(x => x.CurrentLevel == level).ToArray()
            : PlayerHandler.Players.Values.ToArray();
        playerList = playerList.Where(x => x.ClientID != excludePlayerId).ToArray();
        if (playerList.Length == 0)
            return null;
        var randomIndex = new Random().Next(playerList.Length);
        return playerList[randomIndex];
    }

    [MessageHandler((ushort)MessageID.AdvancedTeleport)]
    private static void HandleTeleportRequest(ushort fromClientId, Message message)
    {
        if (!message.GetBool())
            Teleport(message.GetBool(), message.GetUShort(), message.GetBool(), message.GetUShort(), fromClientId);
        else
            Teleport(message.GetBool(), message.GetUShort(), message.GetStrings(), fromClientId);
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