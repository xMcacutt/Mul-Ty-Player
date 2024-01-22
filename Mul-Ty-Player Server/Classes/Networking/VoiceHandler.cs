using System;
using System.Numerics;
using MulTyPlayer;
using NAudio.Wave;

namespace MulTyPlayerServer;
using Riptide;

public class VoiceHandler
{
    [MessageHandler((ushort)MessageID.Voice)]
    private static void VoiceDataReceived(ushort fromClientId, Message message)
    {
        if (!PlayerHandler.Players.TryGetValue(fromClientId, out var incomingPlayer))
            return;
        var decodedAudioData = message.GetBytes();
        foreach (var outgoingPlayer in PlayerHandler.Players.Values)
        {
            if (incomingPlayer.ClientID == outgoingPlayer.ClientID)
                continue;
            var relay = Message.Create(MessageSendMode.Unreliable, MessageID.Voice);
            relay.AddBytes(decodedAudioData);
            relay.AddUShort(fromClientId);
            var incomingClientPos = new Vector3(incomingPlayer.Coordinates[0], incomingPlayer.Coordinates[1], incomingPlayer.Coordinates[2]);
            var outgoingClientPos = new Vector3(outgoingPlayer.Coordinates[0], outgoingPlayer.Coordinates[1], outgoingPlayer.Coordinates[2]);
            var distance = Vector3.Distance(incomingClientPos, outgoingClientPos);
            relay.AddFloat(distance);
            Server._Server.SendToAll(relay, fromClientId);
        }
    }
}