using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking.Voice;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using NAudio.Codecs;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Riptide;

namespace MulTyPlayerClient;

public static class VoiceHandler
{
    private static WaveInEvent _waveIn;
    private static IWavePlayer _waveOut;
    private static MixingSampleProvider _mixer;
    public static bool DoProximityCheck;
    private static int _inputDeviceIndex;
    private const ushort THRESHOLD = 0x0050;
    private const float RANGE_LOWER_BOUND = 200f;
    private const int SAMPLE_RATE = 16000;
    private const int BIT_DEPTH = 16;
    private const int BUFFER_DURATION = 20;
    private static WaveFormat _format = new WaveFormat(SAMPLE_RATE, BIT_DEPTH, 1);
    private static Dictionary<ushort, Voice> _voices;
    private static int _sequenceNumber; // Track the sequence number for each client

    public static void HandleVoiceData(ushort fromClientId, ulong originalLength, float distance, int level, byte[] data)
    {
        var decodedBytes = data; // This should already be decoded from the server
        _voices ??= new Dictionary<ushort, Voice>();
        if (!_voices.TryGetValue(fromClientId, out var voice))
            AddVoice(fromClientId);
        voice = _voices[fromClientId];

        if (DoProximityCheck)
        {
            if (level != Client.HLevel.CurrentLevelId)
                return;
            if (PlayerHandler.TryGetPlayer(fromClientId, out var player))
            {
                voice.SampleChannel.Volume = distance >= SettingsHandler.ClientSettings.ProximityRange ? 0.0f :
                    distance <= RANGE_LOWER_BOUND ? 1.0f :
                    1.0f - (distance - RANGE_LOWER_BOUND) / (SettingsHandler.ClientSettings.ProximityRange - RANGE_LOWER_BOUND);
            }
        }
        
        voice.WaveProvider.AddSamples(decodedBytes, 0, decodedBytes.Length);
    }

    public static void AddVoice(ushort clientId)
    {
        _voices ??= new Dictionary<ushort, Voice>();
        TryRemoveVoice(clientId);
        _voices.Add(clientId, new Voice(_format));
        _mixer.AddMixerInput(_voices[clientId].SampleChannel);
    }

    public static void TryRemoveVoice(ushort clientId)
    {
        _voices ??= new Dictionary<ushort, Voice>();
        if (!_voices.TryGetValue(clientId, out var voice))
            return;
        voice.WaveProvider.ClearBuffer();
        _voices.Remove(clientId);
    }

    private static void ClearVoices()
    {
        _voices ??= new Dictionary<ushort, Voice>();
        foreach (var voice in _voices.Keys.ToList())
            TryRemoveVoice(voice);
        _voices.Clear();
    }

    public static void JoinVoice()
    {
        _waveIn = new WaveInEvent
        {
            DeviceNumber = _inputDeviceIndex,
            WaveFormat = _format,
            BufferMilliseconds = BUFFER_DURATION
        };
        _waveIn.DataAvailable += WaveIn_DataAvailable;
        VoiceClient.OpenVoiceSocket(Client._ip);
        _waveIn.StartRecording();
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, 1));
        _waveOut = new WaveOut();
        _waveOut.Init(_mixer);
        _waveOut.Play();
    }

    public static void LeaveVoice()
    {
        if (_waveIn == null) 
            return;

        VoiceClient.CloseVoiceSocket();
    
        _waveIn.StopRecording();
        _waveIn.Dispose();
        _waveIn = null;
    
        ClearVoices();
    }

    static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        try
        {
            // Send audio data along with the current sequence number
            VoiceClient.SendAudio(e.Buffer, e.Buffer.Length, GetNextSequenceNumber());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void UpdateInputDevice(int index)
    {
        _inputDeviceIndex = index;
    }

    private static int _nextSequenceNumber = 0;
    private const int MaxSequenceNumber = 65535; // Max value for ushort
    private static int GetNextSequenceNumber()
    {
        // Increment and wrap around if necessary
        _nextSequenceNumber++;
        if (_nextSequenceNumber > MaxSequenceNumber)
        {
            _nextSequenceNumber = 0; // Reset if exceeding the maximum
        }
        return _nextSequenceNumber;
    }
    
    private static byte[] ProcessVoiceData(byte[] inputData, float gain, ushort threshold)
    {
        var ushortCount = inputData.Length / 2;
        var ushortArray = new ushort[ushortCount];

        unsafe
        {
            fixed (byte* inputBytesPtr = inputData)
                fixed (ushort* ushortArrayPtr = ushortArray)
                    Buffer.MemoryCopy(inputBytesPtr, ushortArrayPtr, inputData.Length, inputData.Length);
        }

        for (var i = 0; i < ushortCount; i++)
            ushortArray[i] = (ushort)Math.Max(ushortArray[i] * gain, threshold);

        var outputBytes = new byte[inputData.Length];
        unsafe
        {
            fixed (ushort* ushortArrayPtr = ushortArray)
                fixed (byte* outputBytesPtr = outputBytes)
                    Buffer.MemoryCopy(ushortArrayPtr, outputBytesPtr, outputBytes.Length, outputBytes.Length);
        }
        return outputBytes;
    }
}

