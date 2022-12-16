using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ProcessHandler.WriteData(address, new byte[] { 1 });
            //Console.WriteLine("writing to " + Enum.GetValues(typeof(RCData)).GetValue(iAttribute));
        }

        public override void Sync(int null1, byte[] bytes)
        {
            int address = SyncHandler.SaveDataBaseAddress + 0xA84;
            ProcessHandler.WriteData(address, bytes);
        }
    }
}
