using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandTeleport
{

    private static void Teleport(bool fromIsClientId, ushort from, bool toIsClientId, ushort to, ushort sender)
    {
        var fromPlayer = GetPlayer(fromIsClientId, from, SelectorType.From);
        if (fromPlayer is null)
        {
            PeerMessageHandler.SendMessageToClient("[ERROR] Teleport failed, could not find player", false, sender);
            return;
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
                if (player.ClientID == fromPlayer.ClientID)
                    continue;
                var allMessage = Message.Create(MessageSendMode.Reliable, MessageID.AdvancedTeleport);
                allMessage.AddUShort(toPlayer.ClientID);
                Server._Server.Send(allMessage, player.ClientID);
            }
            return;
        }

        if (fromPlayer.ClientID == toPlayer.ClientID && !fromIsClientId && (Selector)from == Selector.RandomPlayer)
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
    RandomPlayer
}

public enum SelectorType : ushort
{
    From,
    To
}