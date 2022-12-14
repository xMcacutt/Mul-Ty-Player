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
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * index), new byte[] { HSyncObject.WriteState });
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
            byte[] buffer = new byte[1];
            int bytesRead = 0;
            for (int i = 0; i < HSyncObject.ObjectAmount; i++)
            {
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), address + StateOffset + (ObjectLength * i), buffer, 1, ref bytesRead);
                currentData[i] = buffer[0];
            }
            Console.WriteLine();
            return currentData;
        }
    }
}
