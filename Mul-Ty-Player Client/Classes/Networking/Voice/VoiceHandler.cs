using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking.Voice;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using NAudio.Codecs;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Riptide;
using RNNoiseSharp;

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
    private static ConcurrentDictionary<ushort, Voice> _voices;
    private static int _sequenceNumber; // Track the sequence number for each client

    public static Compressor Compressor;
    public static NoiseGate NoiseGate;
    public static InputGain InputGain;
    public static OutputGain OutputGain;
    //public static Denoiser Denoiser;
    
    private static bool muted;
    public static event Action<bool> OnMutedChanged;
    public static bool Muted
    {
        get => muted;
        set
        {
            muted = value;
            OnMutedChanged(Muted);
        }
    }
    
    public static void HandleVoiceData(ushort fromClientId, ulong originalLength, float distance, int level, byte[] data)
    {
        _voices ??= new ConcurrentDictionary<ushort, Voice>();
        if (!_voices.TryGetValue(fromClientId, out var voice))
            AddVoice(fromClientId);
        voice = _voices[fromClientId];
        
        if (DoProximityCheck)
        {
            if (level != Client.HLevel.CurrentLevelId)
                return;
            voice.SampleChannel.Volume = distance >= SettingsHandler.ClientSettings.ProximityRange ? 0.0f :
                distance <= RANGE_LOWER_BOUND ? 1.0f :
                1.0f - (distance - RANGE_LOWER_BOUND) / (SettingsHandler.ClientSettings.ProximityRange - RANGE_LOWER_BOUND);
        }
        else
            voice.SampleChannel.Volume = 1.0f;


        if (PlayerHandler.TryGetPlayer(fromClientId, out var player))
        {
            var sampleLevel = Math.Abs(BitConverter.ToInt16(data.AsSpan()[..2]));
            if (sampleLevel > 1000 && !player.IsTalking)
                player.IsTalking = true;
            if (sampleLevel < 250 && player.IsTalking || voice.SampleChannel.Volume == 0)
                player.IsTalking = false;
        }

        voice.WaveProvider.AddSamples(data, 0, data.Length);
    }

    public static void AddVoice(ushort clientId)
    {
        _voices ??= new ConcurrentDictionary<ushort, Voice>();
        TryRemoveVoice(clientId);
        _voices.TryAdd(clientId, new Voice(_format));
        _mixer.AddMixerInput(_voices[clientId].SampleChannel);
        if (PlayerHandler.TryGetPlayer(clientId, out var player))
            player.IsTalking = false;
    }

    public static void TryRemoveVoice(ushort clientId)
    {
        _voices ??= new ConcurrentDictionary<ushort, Voice>();
        if (!_voices.TryGetValue(clientId, out var voice))
            return;
        voice.WaveProvider.ClearBuffer();
        _voices.Remove(clientId, out _);
    }

    private static void ClearVoices()
    {
        _voices ??= new ConcurrentDictionary<ushort, Voice>();
        foreach (var voice in _voices.Keys.ToList())
            TryRemoveVoice(voice);
        _voices.Clear();
    }

    public static void JoinVoice()
    {
        VoiceClient.OpenVoiceSocket(Client._ip);
        Compressor = new Compressor();
        NoiseGate = new NoiseGate();
        InputGain = new InputGain();
        OutputGain = new OutputGain();
        //Denoiser = new Denoiser();
        _waveIn = new WaveInEvent
        {
            DeviceNumber = _inputDeviceIndex,
            WaveFormat = _format,
            BufferMilliseconds = BUFFER_DURATION
        };
        _waveIn.DataAvailable += WaveIn_DataAvailable;
        _waveIn.StartRecording();
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, 1));
        _waveOut = new WaveOut();
        _waveOut.Init(_mixer);
        Thread.Sleep(500);
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
        _waveOut.Stop();
        _waveOut.Dispose();
        _waveOut = null;

        ClearVoices();

        foreach (var player in PlayerHandler.Players)
            player.IsTalking = false;
    }

    static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        try
        {
            if (Muted)
                return;
            VoiceClient.SendAudio(ProcessVoiceData(e.Buffer), e.Buffer.Length, GetNextSequenceNumber());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void UpdateInputDevice(int index)
    {
        _inputDeviceIndex = index;

        if (_waveIn == null || !VoiceClient._isListening)
            return;

        _waveIn?.StopRecording();
        _waveIn?.Dispose();
        _waveIn = new WaveInEvent
        {
            DeviceNumber = _inputDeviceIndex,
            WaveFormat = _format,
            BufferMilliseconds = BUFFER_DURATION
        };

        _waveIn.DataAvailable += WaveIn_DataAvailable;
        _waveIn.StartRecording();
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
    
    private static byte[] ProcessVoiceData(byte[] inputData)
    {
        var shortCount = inputData.Length / 2;
        float[] floatBuffer = new float[shortCount];
        byte[] outputBytes = new byte[inputData.Length];
        
        Parallel.For(0, shortCount, i =>
        {
            floatBuffer[i] = BitConverter.ToInt16(inputData, i * 2) / (float)short.MaxValue;
        });

        // Apply RNNoise denoising in place
        //Denoiser.Denoise(floatBuffer);

        // Process the float data and convert back to bytes in parallel
        Parallel.For(0, shortCount, i =>
        {
            float sample = floatBuffer[i];

            // Apply effects
            sample = Compressor.ApplyCompression(sample);

            // Clamp and convert to 16-bit integer
            int clampedSample = (int)(sample * short.MaxValue);
            clampedSample = Math.Clamp(clampedSample, short.MinValue, short.MaxValue);

            // Write the 16-bit integer to the output byte array
            outputBytes[i * 2] = (byte)clampedSample;
            outputBytes[i * 2 + 1] = (byte)(clampedSample >> 8);
        });

        return outputBytes;
    }

    public static void UpdateEffectsSettings()
    {
        InputGain.Gain = SettingsHandler.ClientSettings.IgGain;
        OutputGain.Gain = SettingsHandler.ClientSettings.OgGain;
        Compressor.InputGain = SettingsHandler.ClientSettings.CmpInputGain;
        Compressor.Threshold = SettingsHandler.ClientSettings.CmpThreshold;
        Compressor.Ratio = SettingsHandler.ClientSettings.CmpRatio;
        Compressor.OutputGain = SettingsHandler.ClientSettings.CmpOutputGain;
        NoiseGate.NoiseFloor = SettingsHandler.ClientSettings.NsGtFloor;
        NoiseGate.NoiseCeiling = SettingsHandler.ClientSettings.NsGtCeiling;
    }

}

