﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient.Classes.Networking.Voice;

public class VoiceClient
{
    private static UdpClient _voiceClient;
    private static IPEndPoint _endPoint;
    private static IPAddress _ip;
    private static ushort _port;
    public static bool _isListening;
    private static CancellationTokenSource _cts;
    
    public static void OpenVoiceSocket(string fullIp)
    {
        CloseVoiceSocket(); 
        try
        {
            if (fullIp.Contains(':'))
            {
                if (!IPAddress.TryParse(fullIp.Split(':')[0], out _ip))
                    throw new Exception();
                if (!ushort.TryParse(fullIp.Split(':')[1], out _port))
                    _port = SettingsHandler.ClientSettings.Port;
            }
            else
            {
                if (!IPAddress.TryParse(fullIp, out _ip))
                    throw new Exception();
                _port = SettingsHandler.ClientSettings.Port;
            }
            _port += 1;
            _voiceClient = new UdpClient();
            _endPoint = new IPEndPoint(IPAddress.Any, 0);
            _voiceClient.Client.Bind(_endPoint);
            _voiceClient.Connect(_ip, _port);
            ModelController.Lobby.IsVoiceConnected = true;
            Logger.Write("Connected to MTP Voice");
            _isListening = true;
            _cts = new CancellationTokenSource();
            
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
        CancellationToken cancellationToken = _cts.Token;
        while (_isListening && !cancellationToken.IsCancellationRequested)
        {
            if (_voiceClient == null) 
                return;
            try
            {
                var data = _voiceClient.Receive(ref _endPoint);
                if (data.Length > 0)
                    ReceiveAudio(data);
            }
            catch (SocketException ex)
            {
                break;
            }
        }
    }

    public static void CloseVoiceSocket()
    {
        _isListening = false;
        _voiceClient?.Send(new byte[] { 0xFF }.Concat(BitConverter.GetBytes(Client._client.Id)).ToArray(), 3);
        _cts?.Cancel();
        _voiceClient?.Close();
        _voiceClient = null;
        ModelController.Lobby.IsVoiceConnected = false;
    }

    public static void SendAudio(IEnumerable<byte> data, long originalDataLength, int sequenceNumber)
    {
        var bytes = BitConverter.GetBytes(Client._client.Id)
            .Concat(BitConverter.GetBytes(originalDataLength))
            .Concat(BitConverter.GetBytes(sequenceNumber)) // Include sequence number
            .Concat(data).ToArray();
        try
        {
            _voiceClient?.Send(bytes, bytes.Length);
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
        VoiceHandler.HandleVoiceData(fromClientId, originalLength, distance, level, audioData);
    }
}