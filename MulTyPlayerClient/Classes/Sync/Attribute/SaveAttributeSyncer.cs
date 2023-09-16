using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
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
            bool saved = ProcessHandler.WriteData(address, new byte[] { 1 }, $"Setting attribute {Enum.GetValues(typeof(Attributes)).GetValue(iAttribute)} to true");
            if (saved)
            {
                SyncHandler.HAttribute.GlobalObjectData[iAttribute] = 1;
            }
        }

        public override void Sync(int null1, byte[] bytes)
        {
            for(int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 1)//&& SyncHandler.HAttribute.GlobalObjectData[i] == 0)
                {                    
                    Save(i, null);
                }
            }
        }
    }
}
