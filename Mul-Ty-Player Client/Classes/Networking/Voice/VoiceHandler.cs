using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using LZ4;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking.Voice;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Riptide;

namespace MulTyPlayerClient;

public class VoiceHandler
{
    private static WaveInEvent _waveIn;
    private const ushort THRESHOLD = 0x2000;
    public static bool DoProximityCheck;
    private const float RANGE_UPPER_BOUND = 3500f;
    private const float RANGE_LOWER_BOUND = 500f;
    private const int SAMPLE_RATE = 44100;
    private const int BIT_DEPTH = 16;
    private const int BUFFER_DURATION = 15;
    private static Dictionary<ushort, Voice> _voices;
    
    public static void HandleVoiceData(ushort fromClientId, ulong originalLength, float distance, int level, byte[] data)
    {
        var decodedBytes = LZ4Codec.Decode(data, 0, data.Length, (int)originalLength);
        _voices ??= new Dictionary<ushort, Voice>();
        if (!_voices.TryGetValue(fromClientId, out var voice))
            return;
        try
        {
            if (DoProximityCheck)
            {
                if (level != Client.HLevel.CurrentLevelId)
                    return;
                var playerInfo = ModelController.Lobby.PlayerInfoList.FirstOrDefault(x => x.ClientID == fromClientId);
                if (playerInfo != null)
                {
                    if (playerInfo.Level != "M/L" || !Client.HGameState.IsAtMainMenu())
                        voice.SampleChannel.Volume = distance >= RANGE_UPPER_BOUND ? 0.0f :
                            distance <= RANGE_LOWER_BOUND ? 1.0f :
                            1.0f - (distance - RANGE_LOWER_BOUND) / (RANGE_UPPER_BOUND - RANGE_LOWER_BOUND);
                }
            }
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
        _voices.Add(clientId, new Voice(new WaveFormat(SAMPLE_RATE, BIT_DEPTH, 1)));
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
            WaveFormat = new WaveFormat(SAMPLE_RATE, BIT_DEPTH, 1),
            BufferMilliseconds = BUFFER_DURATION
        };
        _waveIn.DataAvailable += WaveIn_DataAvailable;
        VoiceClient.OpenVoiceSocket(Client._ip);
        _waveIn.StartRecording();
    }

    public static void StopCaptureVoice()
    {
        if (_waveIn == null) return;
        VoiceClient.CloseVoiceSocket();
        _waveIn.StopRecording();
        _waveIn.Dispose();
        _waveIn = null;
    }

    static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        try
        {
            var encodedBytes = LZ4Codec.Encode(e.Buffer, 0, e.Buffer.Length);
            VoiceClient.SendAudio(encodedBytes, e.Buffer.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

