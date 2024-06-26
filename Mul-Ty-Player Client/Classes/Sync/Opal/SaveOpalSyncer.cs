﻿namespace MulTyPlayerClient;

internal class SaveOpalSyncer : SaveDataSyncer
{
    public override void Save(int index, int? level)
    {
        var crateOpals = Levels.GetLevelData((int)level).CrateOpalCount;
        var newIndex = index > 299 - crateOpals ? 300 - crateOpals + (299 - index) : index;
        var byteIndex = newIndex / 8 + 1;
        var bitIndex = newIndex % 8;
        var address = SyncHandler.SaveDataBaseAddress + 0x70 * (int)level + byteIndex;
        ProcessHandler.TryRead(address, out byte b, false, "SaveOpalSyncer::Save()");
        b |= (byte)(1 << bitIndex);
        ProcessHandler.WriteData(address, new[] { b }, "Setting new opal save data byte value");
    }

    public override void Sync(int level, byte[] data)
    {
        for (var i = 0; i < data.Length; i++)
            if (data[i] == 1 && SyncHandler.HOpal.GlobalObjectData[level][i] != 5)
                Save(i, level);
    }
}