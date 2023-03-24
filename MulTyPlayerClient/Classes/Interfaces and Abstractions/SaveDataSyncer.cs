using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal abstract class SaveDataSyncer
    {
        public int SaveDataOffset { get; set; }
        public byte SaveWriteValue { get; set; }

        public virtual void Save(int iSave, int? level) 
        {
            int address = (int)(SyncHandler.SaveDataBaseAddress + (SaveDataOffset) + (0x70 * level) + iSave);
            ProcessHandler.WriteData(address, new byte[] { SaveWriteValue }, "Saving collectible to save data");
        }

        public virtual void Sync(int level, byte[] bytes) 
        {
            int address = SyncHandler.SaveDataBaseAddress + SaveDataOffset + (0x70 * level);
            ProcessHandler.WriteData(address, bytes, "Syncing all collectible save data for a single collectible");
        }


    }
}
