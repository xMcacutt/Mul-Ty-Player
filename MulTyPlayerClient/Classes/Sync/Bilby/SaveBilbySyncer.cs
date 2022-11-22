using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SaveBilbySyncer : SaveDataSyncer
    {
        public SaveBilbySyncer()
        {
            SaveDataOffset = 0x3A;
            SaveWriteValue = 1;
        }
    }
}
