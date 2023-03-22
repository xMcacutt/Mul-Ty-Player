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

        public async virtual Task Collect(int index)
        {
            if (HSyncObject.CurrentObjectData[index] >= 3) return;
            if (await Client.HGameState.CheckMenuOrLoading()) return;
            await ProcessHandler.WriteDataAsync(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * index), new byte[] { HSyncObject.WriteState });
            if (!SeparateCollisionByte) return;
            await ProcessHandler.WriteDataAsync(HSyncObject.LiveObjectAddress + CollisionOffset + (ObjectLength * index), BitConverter.GetBytes(0));
        }

        public async virtual Task Sync(byte[] bytes, int amount, int checkState)
        {
            for (int i = 0; i < amount; i++)
            {
                if (bytes[i] == checkState) await Collect(i);
            }
        }

        public async virtual Task<byte[]> ReadData()
        {
            byte[] currentData = new byte[HSyncObject.ObjectAmount];
            int address = HSyncObject.LiveObjectAddress;
            for (int i = 0; i < HSyncObject.ObjectAmount; i++)
            {
                currentData[i] = (await ProcessHandler.ReadDataAsync(address + StateOffset + (ObjectLength * i), 1))[0];
            }
            return currentData;
        }
    }
}
