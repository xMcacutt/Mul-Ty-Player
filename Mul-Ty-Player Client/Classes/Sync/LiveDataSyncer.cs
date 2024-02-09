using System;

namespace MulTyPlayerClient;

internal abstract class LiveDataSyncer
{
    public SyncObjectHandler HSyncObject { get; set; }
    public int StateOffset { get; set; }
    public bool SeparateCollisionByte { get; set; }
    public int CollisionOffset { get; set; }
    public int ObjectLength { get; set; }

    public virtual void Collect(int index)
    {
        if (HSyncObject.CurrentObjectData[index] >= 3) return;
        if (Client.HGameState.IsOnMainMenuOrLoading) return;
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + ObjectLength * index,
            new[] { HSyncObject.WriteState }, "Setting collectible to collected");
        if (!SeparateCollisionByte) return;
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + CollisionOffset + ObjectLength * index,
            BitConverter.GetBytes(0), "Setting collision of collectible to off");
    }

    public virtual void Sync(byte[] bytes, int amount, int checkState)
    {
        for (var i = 0; i < amount; i++)
            if (bytes[i] == checkState)
                Collect(i);
    }

    public virtual byte[] ReadData()
    {
        var currentData = new byte[HSyncObject.ObjectAmount];
        var address = HSyncObject.LiveObjectAddress;
        for (var i = 0; i < HSyncObject.ObjectAmount; i++)
            ProcessHandler.TryRead(address + StateOffset + ObjectLength * i, out currentData[i], false,
                "LiveDataSyncer::ReadData()");
        return currentData;
    }
}