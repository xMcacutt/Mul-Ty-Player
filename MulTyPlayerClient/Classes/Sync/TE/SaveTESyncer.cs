using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SaveTESyncer : SaveDataSyncer
    {
        public SaveTESyncer()
        {
            SaveDataOffset = 0x28;
            SaveWriteValue = 1;
        }
    }
}
