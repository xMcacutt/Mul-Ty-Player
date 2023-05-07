using System;

namespace MulTyPlayerClient.Sync
{
    internal class SaveAttributeSyncer : SaveDataSyncer
    {
        public SaveAttributeSyncer()
        {
            SaveWriteValue = 1;
        }

        public override void Save(int iAttribute, int? nullableInt)
        {
            int address = SyncHandler.SaveDataBaseAddress + 0xAA4 + iAttribute;
            Memory.ProcessHandler.WriteData(address, new byte[] { 1 }, $"Setting attribute {Enum.GetValues(typeof(Attributes)).GetValue(iAttribute)} to true");
        }

        public override void Sync(int null1, byte[] bytes)
        {
            int address = SyncHandler.SaveDataBaseAddress + 0xAA4;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 1 && SyncHandler.HAttribute.GlobalObjectData[i] == 0)
                {
                    SyncHandler.HAttribute.GlobalObjectData[i] = 1;
                    Save(i, null);
                }
            }
        }
    }
}
