using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal abstract class LiveDataSyncer
    {
        public SyncObjectHandler HSyncObject { get; set; }
        public int StateOffset { get; set;}
        public bool SeparateCollisionByte { get; set; }
        public int CollisionOffset { get; set; }
        public int ObjectLength { get; set; }

        public virtual void Collect(int index)
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

        public virtual byte[] ReadData()
        {
            byte[] currentData = new byte[HSyncObject.ObjectAmount];
            int address = HSyncObject.LiveObjectAddress;
            for (int i = 0; i < HSyncObject.ObjectAmount; i++)
            {
                currentData[i] = ProcessHandler.ReadData("current object read", address + StateOffset + (ObjectLength * i), 1)[0];
            }
            return currentData;
        }
    }
}
