using System;

namespace MulTyPlayerClient
{
    internal class SaveRCSyncer : SaveDataSyncer
    {

        public SaveRCSyncer()
        {
            SaveWriteValue = 1;    
        }

        public override void Save(int iAttribute, int? nullableInt)
        {
            int address = SyncHandler.SaveDataBaseAddress + 0xA84 + iAttribute;
            ProcessHandler.WriteData(address, new byte[] { 1 }, $"Setting RC data attribute {Enum.GetValues(typeof(RCData)).GetValue(iAttribute)} to true in save data");
        }

        public override void Sync(int null1, byte[] bytes)
        {
            int address = SyncHandler.SaveDataBaseAddress + 0xA84;
            ProcessHandler.WriteData(address, bytes, "Syncing all RC data");
        }
    }
}
