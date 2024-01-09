namespace MulTyPlayerClient;

internal class SaveTESyncer : SaveDataSyncer
{
    public SaveTESyncer()
    {
        SaveDataOffset = 0x28;
        SaveWriteValue = 1;
    }
}