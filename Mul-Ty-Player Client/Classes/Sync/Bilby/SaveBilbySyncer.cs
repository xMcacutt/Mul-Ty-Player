using System.Linq;

namespace MulTyPlayerClient;

internal class SaveBilbySyncer : SaveDataSyncer
{
    public SaveBilbySyncer(BilbyHandler hBilby)
    {
        HBilby = hBilby;
        SaveDataOffset = 0x3A;
        SaveWriteValue = 1;
    }

    private BilbyHandler HBilby { get; }

    public override void Save(int iSave, int? level)
    {
        byte writeState = 1;
        if (HBilby.GlobalObjectData[(int)level].All(x => x == 0))
            writeState = 3;
        var address = (int)(SyncHandler.SaveDataBaseAddress + SaveDataOffset + 0x70 * level + iSave);
        ProcessHandler.WriteData(address, new[] { writeState }, "Saving collectible to save data");
    }
}