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
            SaveDataOffset = 0x1;
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

            if (rem == 0)
            {
                byteIndex += 1;
                if (byteIndex == 1) bitValue = 128;
                else bitValue = 1;
            }
            else
            {
                if (byteIndex == 1) bitValue = (int)Math.Pow(2, rem - 1);
                else bitValue = (int)Math.Pow(2, rem);
            }

            int address = (int)(SyncHandler.SaveDataBaseAddress + SaveDataOffset + (0x70 * level) + (byteIndex - 1));
            byte b = ProcessHandler.ReadData("Save Read", address, 1)[0];
            b += (byte)bitValue;
            Console.WriteLine($"Adding {bitValue} to opal index {index} at byte {byteIndex - 1}");
            ProcessHandler.WriteData(address, new byte[] {b});
        }

        public override void Sync(int level, byte[] data)
        {
            byte[] opalBytes = ConvertOpals(data);
            int address = SyncHandler.SaveDataBaseAddress + SaveDataOffset + (0x70 * level);
            ProcessHandler.WriteData(address, opalBytes);
        }

        static byte[] ConvertOpals(byte[] data)
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
        }
    }
}
