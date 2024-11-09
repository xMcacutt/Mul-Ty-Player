using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;

namespace MulTyPlayerServer;

public class VoiceServer
{
    private static UdpClient _voiceServer;
    private static IPEndPoint _endPoint;
    private static Dictionary<ushort, IPEndPoint> _connectedClients;
    private static Dictionary<ushort, Queue<int>> _clientSequenceNumbers; // Store sequence numbers for each client
    
    public static void OpenVoiceServer()
    {
        _endPoint = new IPEndPoint(IPAddress.Any, 0);
        _voiceServer = new UdpClient(SettingsHandler.ServerSettings.Port + 1);
        _connectedClients = new Dictionary<ushort, IPEndPoint>();
        _clientSequenceNumbers = new Dictionary<ushort, Queue<int>>();

        var loop = new Thread(Loop);
        loop.Start();
    }
    
    private static void Loop()
    {
        while (Server._isRunning)
        {
            var data = _voiceServer.Receive(ref _endPoint);
            if (data.Length > 0)
                ReceiveAudio(data);
        }
        _voiceServer.Close();
    }

    private static void ReceiveAudio(byte[] data)
    {
        if (data.Length == 3 && data[0] == 0xFF)
        {
            var client = BitConverter.ToUInt16(data.Skip(1).Take(2).ToArray());
            _connectedClients.Remove(client);
            _clientSequenceNumbers.Remove(client);
        }
        
        if (data.Length < 14) 
            return;

        var fromClientId = BitConverter.ToUInt16(data.Take(2).ToArray());
        var originalAudioDataLength = BitConverter.ToUInt64(data.Skip(2).Take(8).ToArray());
        var sequenceNumber = BitConverter.ToInt32(data.Skip(10).Take(4).ToArray()); 
        var audioData = data.Skip(14).ToArray();


        if (!_connectedClients.ContainsKey(fromClientId))
        {
            _connectedClients[fromClientId] = _endPoint;
            _clientSequenceNumbers[fromClientId] = new Queue<int>();
        }
        
        // Handle packet sequence
        if (IsValidPacket(fromClientId, sequenceNumber))
        {
            // Call the new HandleData method for processing
            VoiceHandler.HandleData(fromClientId, originalAudioDataLength, audioData);
        }
    }

    private static bool IsValidPacket(ushort clientId, int sequenceNumber)
    {
        // Check for duplicate packets or out-of-order packets
        var sequenceQueue = _clientSequenceNumbers[clientId];

        // Check if the sequence number is a duplicate
        if (sequenceQueue.Contains(sequenceNumber))
        {
            return false; // Duplicate packet, ignore it
        }

        // Check for gaps in the sequence numbers
        if (sequenceQueue.Count > 100) // Limit the number of stored sequence numbers
        {
            sequenceQueue.Dequeue(); // Remove oldest sequence number
        }

        sequenceQueue.Enqueue(sequenceNumber); // Add new sequence number to the queue
        return true; // Valid packet
    }

    public static void SendVoiceData(ushort sender, ulong originalLength, float distance, int level, byte[] audioData, ushort toClientId)
    {
        if (!_connectedClients.ContainsKey(toClientId))
            return; // Ensure the client is connected

        var bytes = BitConverter.GetBytes(sender)
            .Concat(BitConverter.GetBytes(originalLength))
            .Concat(BitConverter.GetBytes(distance))
            .Concat(BitConverter.GetBytes(level))
            .Concat(audioData).ToArray();

        _voiceServer.Send(bytes, bytes.Length, _connectedClients[toClientId]); // Send to the specific client
    }
}