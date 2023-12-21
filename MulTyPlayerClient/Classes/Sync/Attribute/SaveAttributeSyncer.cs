using System;

namespace MulTyPlayerClient;

internal class SaveAttributeSyncer : SaveDataSyncer
{
    public SaveAttributeSyncer()
    {
        SaveWriteValue = 1;
    }

    public override void Save(int iAttribute, int? nullableInt)
    {
        var address = SyncHandler.SaveDataBaseAddress + 0xAA4 + iAttribute;
        var saved = ProcessHandler.WriteData(address, new byte[] { 1 },
            $"Setting attribute {Enum.GetValues(typeof(Attributes)).GetValue(iAttribute)} to true");
        if (saved) SyncHandler.HAttribute.GlobalObjectData[iAttribute] = 1;
    }

    public override void Sync(int null1, byte[] bytes)
    {
        for (var i = 0; i < bytes.Length; i++)
            if (bytes[i] == 1) //&& SyncHandler.HAttribute.GlobalObjectData[i] == 0)
                Save(i, null);
    }
}