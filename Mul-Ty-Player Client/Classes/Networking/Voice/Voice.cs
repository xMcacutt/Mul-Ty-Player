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