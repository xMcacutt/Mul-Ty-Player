using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MulTyPlayerClient.Classes.Networking.Voice;

public class VoiceClient
{
    private static UdpClient _voiceClient;
    private static IPEndPoint _endPoint;
    private static IPAddress _ip;
    private static ushort _port;
    private static bool _isListening;
    
    public static void OpenVoiceSocket(string fullIp)
    {
        try
        {
            if (fullIp.Contains(':'))
            {
                if (!IPAddress.TryParse(fullIp.Split(':')[0], out _ip))
                    throw new Exception();
                if (!ushort.TryParse(fullIp.Split(':')[1], out _port))
                    _port = SettingsHandler.Settings.Port;
            }
            else
            {
                if (!IPAddress.TryParse(fullIp, out _ip))
                    throw new Exception();
                _port = SettingsHandler.Settings.Port;
            }
            _port += 1;
            _voiceClient = new UdpClient();
            _endPoint = new IPEndPoint(IPAddress.Any, 0);
            _voiceClient.Client.Bind(_endPoint);
            
            _voiceClient.Connect(_ip, _port);
            Logger.Write("Connected to MTP Voice");
            
            var loop = new Thread(Loop);
            loop.Start();
        }
        catch (SocketException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void Loop()
    {
        try
        {
            while (true)
            {
                var data = _voiceClient.Receive(ref _endPoint);
                if (data.Length > 0)
                    ReceiveAudio(data);
            }
        }
        catch (ObjectDisposedException)
        {

        }
        catch (SocketException)
        {
            
        }
    }

    public static void CloseVoiceSocket()
    {
        _voiceClient.Close();
        _isListening = false;
    }

    public static void SendAudio(IEnumerable<byte> data, long originalDataLength)
    {
        var bytes = BitConverter.GetBytes(Client._client.Id).Concat(BitConverter.GetBytes(originalDataLength)).Concat(data).ToArray();
        try
        {
            _voiceClient.Send(bytes, bytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void ReceiveAudio(byte[] data)
    {
        if (data.Length < 19)
            return;
        var fromClientId = BitConverter.ToUInt16(data.Take(2).ToArray());
        var originalLength = BitConverter.ToUInt64(data.Skip(2).Take(8).ToArray());
        var distance = BitConverter.ToSingle(data.Skip(10).Take(4).ToArray());
        var level = BitConverter.ToInt32(data.Skip(14).Take(4).ToArray());
        var audioData = data.Skip(18).ToArray();
        //VoiceHandler.HandleVoiceData(fromClientId, originalLength, distance, level, audioData);
    }
}