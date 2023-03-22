using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SaveOpalSyncer : SaveDataSyncer
    {

        public SaveOpalSyncer()
        {
        }

        public async override Task Save(int index, int? level)
        {
            int rem;
            int byteIndex;
            int bitValue;
            int crateOpals = SyncHandler.HOpal.CrateOpalsPerLevel[(int)level];

            if(index > (299 - crateOpals)) index = 300 - crateOpals + (299 - index);

            byteIndex = (int)Math.Ceiling((float)index / 8);
            rem = index % 8;

            bitValue = (int)Math.Pow(2, rem);

            int address = (int)(SyncHandler.SaveDataBaseAddress + (0x70 * level) + (byteIndex));
            byte b = (await ProcessHandler.ReadDataAsync(address, 1))[0];
            b += (byte)bitValue;
            //Console.WriteLine($"Adding {bitValue} to opal index {index} at byte {byteIndex}");
            await ProcessHandler.WriteDataAsync(address, new byte[] {b});
        }

        public async override Task Sync(int level, byte[] data)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if (data[i] == 1 && SyncHandler.HOpal.GlobalObjectData[level][i] != (byte)5)
                {
                    await Save(i, level);
                }
            }
        }
    }
}
