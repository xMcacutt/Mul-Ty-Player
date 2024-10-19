using System;
using System.Numerics;
using MulTyPlayer;
using NAudio.Wave;

namespace MulTyPlayerServer;
using Riptide;

public class VoiceHandler
{
    public static void HandleData(ushort fromClientId, ulong originalAudioDataLength, byte[] audioData)
    {
        // Ensure the player exists in the PlayerHandler
        if (!PlayerHandler.Players.TryGetValue(fromClientId, out var incomingPlayer))
            return;

        // Loop through all players to send audio data
        foreach (var outgoingPlayer in PlayerHandler.Players.Values)
        {
            if (incomingPlayer.ClientID == outgoingPlayer.ClientID)
                continue; // Skip sending to the sender

            if (outgoingPlayer.Coordinates.Length < 3)
                continue; // Ensure valid coordinates

            var incomingClientPos = new Vector3(incomingPlayer.Coordinates[0], incomingPlayer.Coordinates[1], incomingPlayer.Coordinates[2]);
            var outgoingClientPos = new Vector3(outgoingPlayer.Coordinates[0], outgoingPlayer.Coordinates[1], outgoingPlayer.Coordinates[2]);

            // Calculate distance and send voice data
            var distance = Vector3.Distance(incomingClientPos, outgoingClientPos);
            VoiceServer.SendVoiceData(fromClientId, originalAudioDataLength, distance, incomingPlayer.CurrentLevel, audioData, outgoingPlayer.ClientID);
        }
    }
}