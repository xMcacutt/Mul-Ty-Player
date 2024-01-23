using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using LZ4;
using MulTyPlayer;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Riptide;

namespace MulTyPlayerClient.Classes.Networking;

public class VoiceHandler
{
    private static WaveInEvent _waveIn;
    private const float MaxAttenuatedRange = 2500f;
    private const float MinAttenuatedRange = 500f;
    private const int SampleRate = 16000;
    private const int BitDepth = 16;
    private const int BufferMillis = 25;
    private static Dictionary<ushort, Voice> _voices;

    [MessageHandler((ushort)MessageID.Voice)]
    private static void ReceiveVoiceData(Message message)
    {
        var voiceClient = message.GetUShort();
        var voiceDataOriginalLength = message.GetInt();
        var voiceData = message.GetBytes();
        var level = message.GetInt();
        var distance = message.GetFloat();
        var decodedBytes = LZ4Codec.Decode(voiceData, 0, voiceData.Length, (int)voiceDataOriginalLength);
        _voices ??= new Dictionary<ushort, Voice>();
        if (!_voices.TryGetValue(voiceClient, out var voice))
            return;
        try
        {
            if (level != Client.HLevel.CurrentLevelId)
                return;
            voice.SampleChannel.Volume = distance >= MaxAttenuatedRange ? 0.0f :
                distance <= MinAttenuatedRange ? 1.0f :
                1.0f - (distance / MaxAttenuatedRange);
            voice.WaveProvider.AddSamples(decodedBytes, 0, decodedBytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public static void AddVoice(ushort clientId)
    {
        _voices ??= new Dictionary<ushort, Voice>();
        _voices.Add(clientId, new Voice(new WaveFormat(SampleRate, BitDepth, 1)));
        _voices[clientId].WavePlayer.Play();
    }

    public static void RemoveVoice(ushort clientId)
    {
        _voices ??= new Dictionary<ushort, Voice>();
        if (!_voices.TryGetValue(clientId, out var voice))
            return;
        voice.WavePlayer.Stop();
        voice.WavePlayer.Dispose();
        voice.WaveProvider.ClearBuffer();
        _voices.Remove(clientId);
    }

    public static void ClearVoices()
    {
        _voices ??= new Dictionary<ushort, Voice>();
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

public class Voice
{
    public readonly IWavePlayer WavePlayer;
    public readonly BufferedWaveProvider WaveProvider;
    public readonly SampleChannel SampleChannel;
    
    public Voice(WaveFormat waveFormat)
    {
        WavePlayer = new WaveOut();
        WaveProvider = new BufferedWaveProvider(waveFormat);
        WaveProvider.DiscardOnBufferOverflow = true;
        SampleChannel = new SampleChannel(WaveProvider);
        WavePlayer.Init(SampleChannel);
    }
}