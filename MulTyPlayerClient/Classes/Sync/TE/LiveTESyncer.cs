using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{ 
    internal class LiveTESyncer : LiveDataSyncer
    {
        public LiveTESyncer()
        {
            StateOffset = 0xC4;
            SeparateCollisionByte = false;
            ObjectLength = 0x144;
        }
    }
}
