namespace MulTyPlayerClient.Sync
{
    internal abstract class SaveDataSyncer
    {
        public int SaveDataOffset
        {
            get; set;
        }
        public byte SaveWriteValue
        {
            get; set;
        }

        public virtual void Save(int iSave, int? level)
        {
            int address = (int)(SyncHandler.SaveDataBaseAddress + SaveDataOffset + 0x70 * level + iSave);
            Memory.ProcessHandler.WriteData(address, new byte[] { SaveWriteValue }, "Saving collectible to save data");
        }

        public virtual void Sync(int level, byte[] bytes)
        {
            int address = SyncHandler.SaveDataBaseAddress + SaveDataOffset + 0x70 * level;
            Memory.ProcessHandler.WriteData(address, bytes, "Syncing all collectible save data for a single collectible");
        }


    }
}
