namespace MulTyPlayerClient
{
    internal class SaveRSSyncer : SaveDataSyncer
    {
        public SaveRSSyncer()
        {
            SaveDataOffset = 0x11;
            SaveWriteValue = 1;
        }
    }
}
