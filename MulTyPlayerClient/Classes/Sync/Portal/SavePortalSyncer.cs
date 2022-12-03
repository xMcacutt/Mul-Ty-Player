using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SavePortalSyncer : SaveDataSyncer
    {
        public override void Save(int null1, int? level)
        {
            int address = (int)(SyncHandler.SaveDataBaseAddress + (0x70 * level));
            ProcessHandler.WriteData(address, new byte[] { 3 });
        }
        public override void Sync(int null1, byte[] bytes)
        {
            for(int i = 0; i < 7; i++)
            {
                if (bytes[i] == 1) Save(0, PortalHandler.FlakyPortals[i]);
            }
        }
    }
}
