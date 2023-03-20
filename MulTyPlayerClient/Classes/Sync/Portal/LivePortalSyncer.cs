using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class LivePortalSyncer : LiveDataSyncer
    {
        public LivePortalSyncer(PortalHandler HPortal)
        {
            HSyncObject = HPortal;
            StateOffset = 0x9C;
            SeparateCollisionByte = false;
            ObjectLength = 0xB0;
        }

        public async override void Collect(int level)
        {
            if (await Client.HGameState.CheckMenuOrLoading()) return;
            int portalIndex = Array.IndexOf(PortalHandler.LivePortalOrder, level);
            await ProcessHandler.WriteDataAsync(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * portalIndex), new byte[] { HSyncObject.WriteState });
        }
    }
}
