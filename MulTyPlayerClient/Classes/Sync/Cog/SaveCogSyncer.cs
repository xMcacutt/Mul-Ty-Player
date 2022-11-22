using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SaveCogSyncer : SaveDataSyncer
    {
        public SaveCogSyncer()
        {
            SaveDataOffset = 0x30;
            SaveWriteValue = 1;
        }
    }
}
