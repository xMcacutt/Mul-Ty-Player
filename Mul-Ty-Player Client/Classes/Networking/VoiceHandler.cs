using System;
using System.Collections.Generic;
using System.Numerics;
using MulTyPlayer;
using NAudio.Wave;
using Riptide;

namespace MulTyPlayerClient.Classes.Networking;

public class VoiceHandler
{
    private static WaveInEvent _waveIn;
    private const float Range = 1000f;
    private static Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)> _voices;

    [MessageHandler((ushort)MessageID.Voice)]
    private static void ReceiveVoiceData(Message message)
    {
        var decodedBytes = message.GetBytes();
        var voiceClient = message.GetUShort();
        var distance = message.GetFloat();
        if (!_voices.TryGetValue(voiceClient, out var voiceTool))
            return;
        voiceTool.Item2.AddSamples(decodedBytes, 0, decodedBytes.Length);
        voiceTool.Item1.Volume = distance >= Range ? 0.0f :
                                 distance == 0 ? 1.0f :
                                 1.0f - distance / Range;
    }
    
    public static void InitializeVoices()
    {
        _voices = new Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)>();
    }

    public static void AddVoice(ushort clientId)
    {
        _voices.Add(clientId, (new WaveOut(), new BufferedWaveProvider(new WaveFormat(16000, 16, 1))));
        _voices[clientId].Item1.Init(_voices[clientId].Item2);
        _voices[clientId].Item1.Play();
    }

    public static void RemoveVoice(ushort clientId)
    {
        _voices[clientId].Item1.Stop();
        _voices[clientId].Item1.Dispose();
        _voices[clientId].Item2.ClearBuffer();
        _voices.Remove(clientId);
    }
    
    public static void StartCaptureVoice()
    {
        _waveIn = new WaveInEvent {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(16000, 16, 1),
            BufferMilliseconds = 10 
        };
        _waveIn.DataAvailable += WaveIn_DataAvailable;
        _waveIn.StartRecording();
    }
    
    public static void StopCaptureVoice()
    {
        if (_waveIn == null) return;
        _waveIn.StopRecording();
        _waveIn.Dispose();
        _waveIn = null;
    }

    static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        try
        {
            var message = Message.Create(MessageSendMode.Unreliable, MessageID.Voice);
            message.AddBytes(e.Buffer);
            Client._client.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}