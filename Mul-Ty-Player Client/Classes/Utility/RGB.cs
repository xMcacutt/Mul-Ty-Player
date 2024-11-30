using System;

namespace MulTyPlayerClient.Classes.Utility;

public class RGB
{
    private readonly float r;
    private readonly float g;
    private readonly float b;
    
    public RGB(float r, float g, float b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public byte[] GetBytes()
    {
        var data = new byte[12];
        BitConverter.GetBytes(r).CopyTo(data, 0);
        BitConverter.GetBytes(g).CopyTo(data, 4);
        BitConverter.GetBytes(b).CopyTo(data, 8);
        return data;
    }
}