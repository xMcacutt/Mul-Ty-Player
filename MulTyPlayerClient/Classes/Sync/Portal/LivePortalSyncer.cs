using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class LivePortalSyncer : LiveDataSyncer
    {
        int[] LivePortalOrder = { 7, 5, 4, 13, 10, 23, 20, 19, 9, 21, 22, 12, 8, 6, 14, 15 };

        public LivePortalSyncer(PortalHandler HPortal)
        {
            HSyncObject = HPortal;
            StateOffset = 0x9C;
            SeparateCollisionByte = false;
            ObjectLength = 0xB0;
        }

        public override void Collect(int portal)
        {
            PortalHandler HPortal = HSyncObject as PortalHandler;
            if (HPortal.portalsActive[portal]) return;
            if (Program.HGameState.CheckMenuOrLoading()) return;
            HPortal.portalsActive[portal] = true;
            int portalIndex = Array.IndexOf(LivePortalOrder, portal);
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * portalIndex), new byte[] { HSyncObject.WriteState });
        }

        public override void Sync(byte[] bytes, int amount, int checkState)
        {
            PortalHandler HPortal = HSyncObject as PortalHandler;
            for (int i = 0; i < 7; i++)
            {
                if (!HPortal.portalsActive[i]) Collect(i);
            }
        }
    }
}
