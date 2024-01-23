using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using LZ4;
using MulTyPlayer;
using NAudio.Wave;
using Riptide;

namespace MulTyPlayerClient.Classes.Networking;

public class VoiceHandler
{
    private static WaveInEvent _waveIn;
    private const float Range = 1000f;
    private const int SampleRate = 11025;
    private const int BitDepth = 16;
    private const int BufferMillis = 25;
    private static Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)> _voices;

    [MessageHandler((ushort)MessageID.Voice)]
    private static void ReceiveVoiceData(Message message)
    {
        var voiceClient = message.GetUShort();
        var voiceDataOriginalLength = message.GetInt();
        var voiceData = message.GetBytes();
        var level = message.GetInt();
        var distance = message.GetFloat();
        var decodedBytes = LZ4Codec.Decode(voiceData, 0, voiceData.Length, (int)voiceDataOriginalLength);
        _voices ??= new Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)>();
        if (!_voices.TryGetValue(voiceClient, out var voiceTool))
            return;
        try
        {
            if (level != Client.HLevel.CurrentLevelId)
                return;
            voiceTool.Item2.AddSamples(decodedBytes, 0, decodedBytes.Length);
            Console.WriteLine(voiceTool.Item1.Volume);
            voiceTool.Item1.Volume = distance >= Range ? 0.0f :
                                     distance == 0 ? 1.0f :
                                     1.0f - (distance / Range);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            voiceTool.Item2.ClearBuffer();
        }
    }

    public static void AddVoice(ushort clientId)
    {
        _voices ??= new Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)>();
        _voices.Add(clientId,
            (new DirectSoundOut(), new BufferedWaveProvider(new WaveFormat(SampleRate, BitDepth, 1))));
        _voices[clientId].Item1.Init(_voices[clientId].Item2);
        _voices[clientId].Item1.Play();
    }

    public static void RemoveVoice(ushort clientId)
    {
        _voices ??= new Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)>();
        if (!_voices.TryGetValue(clientId, out var voice))
            return;
        voice.Item1.Stop();
        voice.Item1.Dispose();
        voice.Item2.ClearBuffer();
        _voices.Remove(clientId);
    }

    public static void ClearVoices()
    {
        _voices ??= new Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)>();
        foreach (var voice in _voices)
            RemoveVoice(voice.Key);
        _voices.Clear();
    }

    public static void StartCaptureVoice()
    {
        _waveIn = new WaveInEvent
        {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(SampleRate, BitDepth, 1),
            BufferMilliseconds = BufferMillis
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
            var encodedBytes = LZ4Codec.Encode(e.Buffer, 0, e.Buffer.Length);
            message.AddBytes(encodedBytes);
            message.AddInt(e.Buffer.Length);
            Client._client.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}