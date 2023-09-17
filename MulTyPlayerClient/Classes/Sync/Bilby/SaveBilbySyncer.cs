namespace MulTyPlayerClient
{
    internal class SaveBilbySyncer : SaveDataSyncer
    {
        public SaveBilbySyncer()
        {
            SaveDataOffset = 0x3A;
            SaveWriteValue = 1;
        }
    }
}
