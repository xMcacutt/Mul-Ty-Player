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
            SaveDataOffset = 0x11;
        }

        public override void Save(int index, int? level)
        {
            int rem;
            int byteIndex;
            int bitValue;

            byteIndex = (int)Math.Ceiling((float)index / 8);
            rem = index % 8;

            if (rem == 0) bitValue = 128;
            else bitValue = (int)Math.Pow(2, rem - 1);

            int address = (int)(SyncHandler.SaveDataBaseAddress + (SaveDataOffset - 1) + (0x70 * level) + byteIndex);
            byte b = ProcessHandler.ReadData("opal save read", address, 1)[0];
            b += (byte)bitValue;
            ProcessHandler.WriteData(address, BitConverter.GetBytes(b));
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
