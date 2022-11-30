using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SavePortalSyncer : SaveDataSyncer
    {
        public override void Save(int iSave, int? level)
        {
            int address = (int)(SyncHandler.SaveDataBaseAddress + (0x70 * level));
            ProcessHandler.WriteData(address, new byte[] { (byte)iSave });
        }
        public override void Sync(int level, byte[] bytes)
        {
            for(int i = 0; i < 24; i++)
            {
                int address = SyncHandler.SaveDataBaseAddress + (0x70 * i);
                ProcessHandler.WriteData(address, new byte[] { bytes[i] });
            }
        }
    }
}
