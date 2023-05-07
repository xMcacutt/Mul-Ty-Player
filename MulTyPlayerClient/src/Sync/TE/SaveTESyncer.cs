namespace MulTyPlayerClient.Sync
{
    internal class SaveTESyncer : SaveDataSyncer
    {
        public SaveTESyncer()
        {
            SaveDataOffset = 0x28;
            SaveWriteValue = 1;
        }
    }
}
