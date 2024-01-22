using System;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
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
    private const int BufferMillis = 5;
    private static Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)> _voices;
    private static Timer _sendTimer;
    private static List<byte> _voiceBuffer = new List<byte>();

    [MessageHandler((ushort)MessageID.Voice)]
    private static void ReceiveVoiceData(Message message)
    {
        var voiceClient = message.GetUShort();
        var decodedBytes = message.GetBytes();
        /*var distance = message.GetFloat();*/
        if (!_voices.TryGetValue(voiceClient, out var voiceTool))
            return;
        Console.WriteLine("Adding samples");
        voiceTool.Item2.AddSamples(decodedBytes, 0, decodedBytes.Length);
        /*voiceTool.Item1.Volume = distance >= Range ? 0.0f :
                                 distance == 0 ? 1.0f :
                                 1.0f - distance / Range;
        */
    }

    public static void AddVoice(ushort clientId)
    {
        _voices ??= new Dictionary<ushort, (IWavePlayer, BufferedWaveProvider)>();
        _voices.Add(clientId, (new DirectSoundOut(), new BufferedWaveProvider(new WaveFormat(SampleRate, BitDepth, 1))));
        _voices[clientId].Item1.Init(_voices[clientId].Item2);
        _voices[clientId].Item1.Play();
        Console.WriteLine($"Playing voice {clientId}");
    }

    public static void RemoveVoice(ushort clientId)
    {
        _voices[clientId].Item1.Stop();
        _voices[clientId].Item1.Dispose();
        _voices[clientId].Item2.ClearBuffer();
        _voices.Remove(clientId);
    }

    public static void ClearVoices()
    {
        foreach (var voice in _voices)
            RemoveVoice(voice.Key);
        _voices.Clear();
    }
    
    public static void StartCaptureVoice()
    {
        _waveIn = new WaveInEvent {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(SampleRate, BitDepth, 1),
            BufferMilliseconds = BufferMillis 
        };
        _sendTimer = new Timer(10); // Adjust the interval based on your needs
        _sendTimer.Elapsed += SendBufferedData;
        _sendTimer.AutoReset = true;
        _sendTimer.Start();
        _waveIn.DataAvailable += WaveIn_DataAvailable;
        _waveIn.StartRecording();
    }

    private static void SendBufferedData(object sender, ElapsedEventArgs e)
    {
        try
        {
            if (_voiceBuffer.Count == 0)
                return;
            var message = Message.Create(MessageSendMode.Unreliable, MessageID.Voice);
            message.AddBytes(_voiceBuffer.ToArray());
            Client._client.Send(message);
            _voiceBuffer.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
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
            _voiceBuffer.AddRange(e.Buffer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}