using System.Windows.Markup;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

internal class LiveTESyncer : LiveDataSyncer
{
    public LiveTESyncer(TEHandler hThEg)
    {
        HSyncObject = hThEg;
        StateOffset = 0xC4;
        SeparateCollisionByte = false;
        ObjectLength = 0x144;
    }

    public void Spawn(int index)
    {
        if (HSyncObject.CurrentObjectData[index] == 5) return;
        for (var i = 0; i < 8; i++)
        {
            ProcessHandler.TryRead(HSyncObject.LiveObjectAddress + HSyncObject.IDOffset + ObjectLength * i,
                out int result, false, "TE Id Read");
            if (result != 1) continue;
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + ObjectLength * index, new byte[] { 1 },
                "Spawning TE");
            return;

        }
        
    }

    [MessageHandler((ushort)MessageID.SpawnBilbyTE)]
    public static void HandleBilbyTE(Message message)
    {
        if (Client.HLevel.CurrentLevelId != message.GetInt()) 
            return;
        (Client.HSync.SyncObjects["TE"].LiveSync as LiveTESyncer)?.Spawn(1);
    }
}