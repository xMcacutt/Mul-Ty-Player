using System;

namespace MulTyPlayerClient.Classes.GamePlay;

public class SpawnerHandler
{
    public static int GetSpawnerCount()
    {
        ProcessHandler.TryRead(0x259EEC, out int result, true, "getSpawnerCount");
        return result;
    }
    
    public static void SetSpawnerDelay(int index, float delay)
    {
        ProcessHandler.TryRead(0x259EF0, out int baseAddr, true, "getSpawnerAddr");
        ProcessHandler.WriteData(baseAddr + 0x104 * index + 0xD8, BitConverter.GetBytes(delay));
    }
    
    public static void MultiplySpawnerDelay(int index, float multiplier)
    {
        ProcessHandler.TryRead(0x259EF0, out int baseAddr, true, "getSpawnerAddr");
        ProcessHandler.TryRead(baseAddr + 0x104 * index + 0xD8, out float delay, false, "getSpawnerAddr");
        ProcessHandler.WriteData(baseAddr + 0x104 * index + 0xD8, BitConverter.GetBytes(delay * multiplier));
    }
}