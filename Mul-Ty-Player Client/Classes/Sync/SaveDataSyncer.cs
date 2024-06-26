﻿namespace MulTyPlayerClient;

internal abstract class SaveDataSyncer
{
    public int SaveDataOffset { get; set; }
    public byte SaveWriteValue { get; set; }

    public virtual void Save(int iSave, int? level)
    {
        var address = (int)(SyncHandler.SaveDataBaseAddress + SaveDataOffset + 0x70 * level + iSave);
        ProcessHandler.WriteData(address, new[] { SaveWriteValue }, "Saving collectible to save data");
    }

    public virtual void Sync(int level, byte[] bytes)
    {
        var address = SyncHandler.SaveDataBaseAddress + SaveDataOffset + 0x70 * level;
        ProcessHandler.WriteData(address, bytes, "Syncing all collectible save data for a single collectible");
    }
}



