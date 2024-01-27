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