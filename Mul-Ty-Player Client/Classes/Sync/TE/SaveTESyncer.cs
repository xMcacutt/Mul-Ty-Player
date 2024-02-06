using System.Collections.Generic;

namespace MulTyPlayerClient;

internal class SaveTESyncer : SaveDataSyncer
{
    public Dictionary<int, byte[]> GlobalSaveData;
    
    public SaveTESyncer()
    {
        SaveDataOffset = 0x28;
        SaveWriteValue = 1;
        GlobalSaveData = new Dictionary<int, byte[]>();
        foreach (var ld in Levels.MainStages)
            GlobalSaveData.Add(ld.Id, new byte[8]);
    }
    
    public override void Save(int iSave, int? level)
    {
        if (level is null)
            return;
        GlobalSaveData[(int)level][iSave] = 1;
        var address = (int)(SyncHandler.SaveDataBaseAddress + SaveDataOffset + 0x70 * level + iSave);
        ProcessHandler.WriteData(address, new[] { SaveWriteValue }, "Saving collectible to save data");
    }

    public override void Sync(int level, byte[] bytes)
    {
        for (var i = 0; i < bytes.Length; i++)
            GlobalSaveData[level][i] = bytes[i];
        var address = SyncHandler.SaveDataBaseAddress + SaveDataOffset + 0x70 * level;
        ProcessHandler.WriteData(address, bytes, "Syncing all collectible save data for a single collectible");
    }
}