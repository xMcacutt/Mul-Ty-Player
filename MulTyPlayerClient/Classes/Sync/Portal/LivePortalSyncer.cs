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
        //fix this
        public override void Collect(int index)
        {
            if (HSyncObject.CurrentObjectData[index] >= 3) return;
            if (Program.HGameState.CheckMenuOrLoading()) return;
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * index), BitConverter.GetBytes(HSyncObject.WriteState));
            if (!SeparateCollisionByte) return;
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + CollisionOffset + (ObjectLength * index), BitConverter.GetBytes(0));
        }

        public virtual void Sync(byte[] bytes, int amount, int checkState)
        {
            for (int i = 0; i < amount; i++)
            {
                if (bytes[i] == checkState) Collect(i);
            }
        }
    }
}
