using System;

namespace MulTyPlayerClient;

internal class LiveCrateSyncer : LiveDataSyncer
{
    public LiveCrateSyncer(SyncObjectHandler hCrate)
    {
        HSyncObject = hCrate;
        StateOffset = 0x114;
        ObjectLength = 0x1C0;
    }

    public override void Collect(int index)
    {
        var crateAddress = HSyncObject.LiveObjectAddress + index * ObjectLength;
        ProcessHandler.WriteData(crateAddress + 0x48, new byte[] { 0 }, "Setting crate collision to false");
        ProcessHandler.WriteData(crateAddress + 0x114, new byte[] { 0 }, "Setting crate visibility to false");
        ProcessHandler.TryRead(crateAddress + 0x178, out byte opalCount, false, "LiveCrateSyncer::Collect() {1}");
        for (var i = 0; i < opalCount; i++)
        {
            ProcessHandler.TryRead(crateAddress + 0x150 + 4 * i, out IntPtr opalAddress, false,
                "LiveCrateSyncer::Collect() {2}");
            ProcessHandler.TryRead(opalAddress + 0x78, out byte opalState, false, "LiveCrateSyncer::Collect() {3}");
            if (opalState == 0)
                ProcessHandler.WriteData((int)(opalAddress + 0x78), BitConverter.GetBytes(1),
                    $"Spawning opal from crate {i} / {opalCount}");
        }
    }

    public override void Sync(byte[] bytes, int amount, int checkState)
    {
        for (var i = 0; i < amount; i++)
            if (bytes[i] == checkState)
                Collect(i);
    }

    public override byte[] ReadData()
    {
        var crateCount = Levels.GetLevelData(Client.HLevel.CurrentLevelId).CrateCount;
        var currentData = new byte[crateCount];
        var address = HSyncObject.LiveObjectAddress;
        for (var i = 0; i < currentData.Length; i++)
            ProcessHandler.TryRead(address + StateOffset + ObjectLength * i, out currentData[i], false,
                "LiveCrateSyncer::ReadData()");
        return currentData;
    }
}