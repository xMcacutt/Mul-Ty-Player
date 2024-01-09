namespace MulTyPlayerClient;

internal class SaveRSSyncer : SaveDataSyncer
{
    public SaveRSSyncer()
    {
        SaveDataOffset = 0x11;
        SaveWriteValue = 1;
    }

    public override void Save(int index, int? level)
    {
        if (level != Levels.RainbowCliffs.Id)
            return;

        var byteIndex = index / 8 + 1;
        var bitIndex = index % 8;

        var address = SyncHandler.SaveDataBaseAddress + SaveDataOffset + byteIndex;
        ProcessHandler.TryRead(address, out byte b, false, "SaveRainbowScaleSyncer::Save()");
        b |= (byte)(1 << bitIndex);
        ProcessHandler.WriteData(address, new[] { b }, "Setting new rainbow scale save data byte value");
    }

    public override void Sync(int level, byte[] data)
    {
        if (level != Levels.RainbowCliffs.Id)
            return;
        for (var i = 0; i < data.Length; i++)
            if (data[i] == 1 && SyncHandler.HRainbowScale.GlobalObjectData[0][i] != 5)
                Save(i, level);
    }
}