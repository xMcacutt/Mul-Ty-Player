using MulTyPlayerClient.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Sync
{
    internal class OpalObserver
    {
        private static nint LiveOpalCountAddress = Addresses.LiveGemCount;
        private static IntObserver memoryObserver = new IntObserver(LiveOpalCountAddress);

        public OpalObserver()
        {
            memoryObserver.OnValueChange += (i, j) => { CheckOpalStates(); };
        }

        byte[] opalStates = new byte[300];

        private void CheckOpalStates()
        {
            for (int i = 0; i < 300; i++)
            {

            }
        }
    }
}
