using System;
using MulTyPlayerClient.Networking;

namespace MulTyPlayerClient.Sync
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

        public override void Collect(int level)
        {
            if (Replication.HGameState.CheckMenuOrLoading()) return;
            int portalIndex = Array.IndexOf(PortalHandler.LivePortalOrder, level);
            Memory.ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * portalIndex), new byte[] { HSyncObject.WriteState }, "Making portal visible");
        }
    }
}
