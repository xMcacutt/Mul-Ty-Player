using System.Collections.Generic;

namespace Mul_Ty_Player_Updater;

public class TyData
{
    public static string[] TyFileNames =
    {
        "Data_PC.rkv", "Music_PC.rkv", "Override_PC.rkv", "Video_PC.rkv", "OpenAL32.dll", "soft_oal.dll",
        "steam_api.dll", "steam_appid.txt", "TY.exe"
    };

    public static Dictionary<int, byte[]> MagnetBytesOrigin = new()
    {
        {0x138477, new byte[] { 0x75, 0x27 }},
        {0x138490, new byte[] { 0xe8, 0x6b, 0x51, 0xff, 0xff, 0x3b, 0xf0, 0x7d, 0x07 }}
    };
    
    public static Dictionary<int, byte[]> MagnetBytesFixed = new()
    {
        {0x138477, new byte[] { 0x90, 0x90 }},
        {0x138490, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }}
    };
}