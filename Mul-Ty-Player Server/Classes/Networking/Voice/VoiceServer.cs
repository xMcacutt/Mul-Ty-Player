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

    
    public static void OpenVoiceServer()
    {
        _endPoint = new IPEndPoint(IPAddress.Any, 0);
        _voiceServer = new UdpClient(SettingsHandler.Settings.Port + 1);
        _connectedClients = new Dictionary<ushort, IPEndPoint>();

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
        var fromClientId = BitConverter.ToUInt16(data.Take(2).ToArray());
        if (!_connectedClients.ContainsKey(fromClientId))
            _connectedClients.Add(fromClientId, _endPoint);
        if (data.Length < 11)
            return;
        var originalAudioDataLength = BitConverter.ToUInt64(data.Skip(2).Take(8).ToArray());
        var audioData = data.Skip(10).ToArray();
        VoiceHandler.HandleData(fromClientId, originalAudioDataLength, audioData);
    }

    public static void SendVoiceData(ushort sender, ulong originalLength, float distance, int level, byte[] audioData, ushort toClientId)
    {
        if (!_connectedClients.ContainsKey(toClientId))
            return;
        var bytes = BitConverter.GetBytes(sender)
            .Concat(BitConverter.GetBytes(originalLength))
            .Concat(BitConverter.GetBytes(distance))
            .Concat(BitConverter.GetBytes(level))
            .Concat(audioData).ToArray();
        foreach (var client in _connectedClients.Where(client => client.Key != sender))
            _voiceServer.Send(bytes, bytes.Length, client.Value);
    }
}