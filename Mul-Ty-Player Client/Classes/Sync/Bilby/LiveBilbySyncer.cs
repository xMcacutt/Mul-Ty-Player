using System;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

internal class LiveBilbySyncer : LiveDataSyncer
{
    public LiveBilbySyncer(BilbyHandler hBilby)
    {
        HSyncObject = hBilby;
        StateOffset = 0x34;
        SeparateCollisionByte = true;
        CollisionOffset = 0x58;
        ObjectLength = 0x134;
    }

    public override void Collect(int index)
    {
        //if (HSyncObject.CurrentObjectData[index] != 1) return;
        if (Client.HGameState.IsOnMainMenuOrLoading) return;
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + ObjectLength * index,
            new[] { HSyncObject.WriteState }, "Collecting bilby");
        ProcessHandler.WriteData(0x2651BC, BitConverter.GetBytes(0xA0));
        ProcessHandler.WriteData(0x2651B8, BitConverter.GetBytes(0x1));
        if (!SeparateCollisionByte) return;
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + CollisionOffset + ObjectLength * index,
            BitConverter.GetBytes(0), "Setting bilby cage collision to off pt 1");
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + 0x31 + ObjectLength * index, new byte[] { 0, 1 },
            "Setting bilby cage collision to off pt 2");
    }

    [MessageHandler((ushort)MessageID.DespawnAllBilbies)]
    public static void DespawnBilbies(Message message)
    {
        if (Client.HLevel.CurrentLevelId != message.GetInt() 
            || Client.HGameState.IsOnMainMenuOrLoading) 
            return;
        ((LiveBilbySyncer)Client.HSync.SyncObjects["Bilby"].LiveSync).CollectAll();
    }
    
    public void CollectAll()
    {
        for (var i = 0; i < 5; i++)
        {
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + ObjectLength * i,
                new[] { HSyncObject.WriteState }, "Collecting bilby");
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + CollisionOffset + ObjectLength * i,
                BitConverter.GetBytes(0), "Setting bilby cage collision to off pt 1");
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + 0x31 + ObjectLength * i, new byte[] { 0, 1 },
                "Setting bilby cage collision to off pt 2");
            HSyncObject.SaveSync.Save(i, Client.HLevel.CurrentLevelId);
        }
    }
}