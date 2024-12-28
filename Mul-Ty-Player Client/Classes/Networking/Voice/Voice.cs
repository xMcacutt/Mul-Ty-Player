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
                > 1.0f => 1.0f,
                < 0.0f => 0.0f,
                _ => value
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
        if (sample > Threshold)
        {
            var excess = sample - Threshold;
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
                > 1.0f => 1.0f,
                < 0.0f => 0.0f,
                _ => value
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
                > 1.0f => 1.0f,
                < 0.0f => 0.0f,
                _ => value
            };
        }
    }

    public float AttackTime = 0.05f;
    public float ReleaseTime = 1.5f;
    private float currentGain = 1.0f;
    private float targetGain = 1.0f;

    public NoiseGate()
    {
        NoiseFloor = SettingsHandler.ClientSettings.NsGtFloor;
        NoiseCeiling = SettingsHandler.ClientSettings.NsGtCeiling;
    }

    public float ApplyNoiseGate(float sample, int sampleRate)
    {
        targetGain = Math.Clamp((sample - NoiseFloor) / (NoiseCeiling - NoiseFloor), 0.0f, 1.0f);
        var smoothingFactor = 1 - (float)Math.Exp(-1 / ((targetGain < currentGain ? AttackTime : ReleaseTime) * sampleRate));
        currentGain += smoothingFactor * (targetGain - currentGain);
        return sample * currentGain;
    }
    
}

public struct InputGain
{
    private float _gain;
    public float Gain
    {
        get => _gain;
        set
        {
            _gain = value switch
            {
                > 5.0f => 5.0f,
                _ => value
            };
        }
    }

    public InputGain()
    {
        Gain = SettingsHandler.ClientSettings.IgGain;
    }

    public float ApplyInputGain(float sample)
    {
        return sample * Gain;
    }
}

public struct OutputGain
{
    private float _gain;
    public float Gain
    {
        get => _gain;
        set
        {
            _gain = value switch
            {
                > 5.0f => 5.0f,
                _ => value
            };
        }
    }

    public OutputGain()
    {
        Gain = SettingsHandler.ClientSettings.OgGain;
    }

    public float ApplyOutputGain(float sample)
    {
        return sample * Gain;
    }
}