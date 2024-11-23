using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MulTyPlayerClient;

public class Voice
{
    public readonly BufferedWaveProvider WaveProvider;
    public readonly SampleChannel SampleChannel;
    
    public Voice(WaveFormat waveFormat)
    {
        WaveProvider = new BufferedWaveProvider(waveFormat);
        WaveProvider.DiscardOnBufferOverflow = true;
        SampleChannel = new SampleChannel(WaveProvider);
    }
}

public struct Compressor
{
    private float _threshold;
    public float Threshold
    {
        get => _threshold;
        set
        {
            _threshold = value switch
            {
                > 1.0f => short.MaxValue,
                < 0.0f => 0,
                _ => value * short.MaxValue
            };
        }
    }
    public float InputGain;
    public float OutputGain;
    public float Ratio;

    public Compressor()
    {
        Threshold = SettingsHandler.ClientSettings.CmpThreshold;
        InputGain = SettingsHandler.ClientSettings.CmpInputGain;
        OutputGain = SettingsHandler.ClientSettings.CmpOutputGain;
        Ratio = SettingsHandler.ClientSettings.CmpRatio;
    }

    public float ApplyCompression(float sample)
    {
        sample *= InputGain;
        if (Math.Abs(sample) > Threshold)
        {
            var excess = Math.Abs(sample) - Threshold;
            if (sample > 0)
                sample = Threshold + excess / Ratio;
            else
                sample = - (Threshold + excess / Ratio);
        }
        sample *= OutputGain;
        return sample;
    }
}

public struct NoiseGate
{
    private float _noiseFloor;
    public float NoiseFloor
    {
        get => _noiseFloor;
        set
        {
            _noiseFloor = value switch
            {
                > 1.0f => short.MaxValue,
                < 0.0f => 0,
                _ => value * short.MaxValue
            };
        }
    }
    
    private float _noiseCeiling;
    public float NoiseCeiling
    {
        get => _noiseCeiling;
        set
        {
            _noiseCeiling = value switch
            {
                > 1.0f => short.MaxValue,
                < 0.0f => 0,
                _ => value * short.MaxValue
            };
        }
    }

    public NoiseGate()
    {
        NoiseFloor = SettingsHandler.ClientSettings.NsGtFloor;
        NoiseCeiling = SettingsHandler.ClientSettings.NsGtCeiling;
    }

    public float ApplyNoiseGate(float sample)
    {
        if (Math.Abs(sample) > NoiseCeiling || Math.Abs(sample) < NoiseFloor)
            return 0;
        return sample;
    }
}