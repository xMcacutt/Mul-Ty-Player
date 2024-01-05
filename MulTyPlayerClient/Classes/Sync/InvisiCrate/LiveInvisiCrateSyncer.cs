using System;

namespace MulTyPlayerClient;

internal class LiveInvisiCrateSyncer : LiveDataSyncer
{
    public LiveInvisiCrateSyncer(SyncObjectHandler hInvisiCrate)
    {
        HSyncObject = hInvisiCrate;
        StateOffset = 0x114;
        ObjectLength = 0x1DC;
    }

    public override void Collect(int index)
    {
        var crateAddress = HSyncObject.LiveObjectAddress + index * ObjectLength;
        ProcessHandler.WriteData(crateAddress + StateOffset, new byte[] { 0x2 }, "Setting InvisiCrate visibility to false");
        ProcessHandler.WriteData(crateAddress + 0x48, new byte[] { 0x0 }, "Setting InvisiCrate collision to false");
        ProcessHandler.WriteData(crateAddress + 0x1CC, new byte[] { 0x1 }, "Setting Frame handled to true");
        ProcessHandler.TryRead(crateAddress + 0x1D0, out int frameIndex, false, "LiveCrateSyncer::Collect() {1}");
        ProcessHandler.TryRead(crateAddress + 0x1C4, out int frameAddress, false, "LiveCrateSyncer::Collect() {2}");
        ProcessHandler.WriteData(frameAddress + 0x84, BitConverter.GetBytes(frameIndex), "Setting Frame index");
        ProcessHandler.WriteData(frameAddress + 0x89, new byte[] {0x1}, "Setting Frame visibility to true");
    }

    public override void Sync(byte[] bytes, int amount, int checkState)
    {
        for (var i = 0; i < amount; i++)
            if (bytes[i] == checkState)
                Collect(i);
    }

    public override byte[] ReadData()
    {
        var crateCount = Levels.GetLevelData(Client.HLevel.CurrentLevelId).FrameCount;
        var currentData = new byte[crateCount];
        var address = HSyncObject.LiveObjectAddress;
        for (var i = 0; i < currentData.Length; i++)
            ProcessHandler.TryRead(address + StateOffset + ObjectLength * i, out currentData[i], false,
                "LiveCrateSyncer::ReadData()");
        return currentData;
    }
}