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

        public override void Save(int index, int? level)
        {
            int rem;
            int byteIndex;
            int bitValue;
            int crateOpals = SyncHandler.HOpal.CrateOpalsPerLevel[(int)level];

            if(index > (299 - crateOpals))
            {
                index = (300 - crateOpals) + (299 - index);
            }

            byteIndex = (int)Math.Ceiling((float)index / 8);
            rem = index % 8;

            bitValue = (int)Math.Pow(2, rem);

            int address = (int)(SyncHandler.SaveDataBaseAddress + (0x70 * level) + (byteIndex));
            byte[] buffer = new byte[1];
            int bytesRead = 0;
            ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), address, buffer, 1, ref bytesRead);
            byte b = buffer[0];
            b += (byte)bitValue;
            //Console.WriteLine($"Adding {bitValue} to opal index {index} at byte {byteIndex}");
            ProcessHandler.WriteData(address, new byte[] {b});
        }

        public override void Sync(int level, byte[] data)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if (data[i] == 1 && SyncHandler.HOpal.GlobalObjectData[level][i] != (byte)5)
                {
                    Save(i, level);
                }
            }
        }

       /* static byte[] ConvertOpals(byte[] data)
        {
            byte[] output = new byte[(int)Math.Ceiling((double)data.Length / 8)];
            for (int i = 0; i < Math.Ceiling((double)data.Length / 8); i++)
            {
                byte[] b = data.Skip(i * 8).Take(8).ToArray();
                if (data.Skip(i * 8).ToArray().Length % 8 != 0)
                {
                    b = data.Skip(i * 8).Take(4).ToArray().Concat(new byte[4]).ToArray();
                }
                BitArray bits = new BitArray(8);
                for (int j = 0; j < 8; j++)
                {
                    if (b[j] == 1)
                    {
                        bits[j] = true;
                    }
                }
                bits.CopyTo(output, i);
            }
            return output;
        }*/
    }
}
