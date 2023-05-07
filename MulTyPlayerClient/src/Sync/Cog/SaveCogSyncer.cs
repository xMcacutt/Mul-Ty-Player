namespace MulTyPlayerClient.Sync
{
    internal class SaveCogSyncer : SaveDataSyncer
    {
        public SaveCogSyncer()
        {
            SaveDataOffset = 0x30;
            SaveWriteValue = 1;
        }
    }
}
