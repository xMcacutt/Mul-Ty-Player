using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MulTyPlayerClient
{
    internal class LiveBilbySyncer : LiveDataSyncer
    {
        public LiveBilbySyncer(BilbyHandler hBilby)
        {
            HSyncObject = hBilby; 
            StateOffset = 0x34;
            SeparateCollisionByte = true;
            CollisionOffset = 0x58;
            ObjectLength = 0x134;
        }

        public override void Collect(int index)
        {
            if (HSyncObject.CurrentObjectData[index] >= 3) return;
            if (Client.HGameState.CheckMenuOrLoading()) return;
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * index), new byte[] {HSyncObject.WriteState});
            if (!SeparateCollisionByte) return;
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + CollisionOffset + (ObjectLength * index), BitConverter.GetBytes(0));
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + 0x31 + (ObjectLength * index), new byte[] { 0, 1 });
        }
    }
}
