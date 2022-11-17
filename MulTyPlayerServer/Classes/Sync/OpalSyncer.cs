using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer
{
    internal class OpalSyncer
    {
        public Dictionary<int, byte[]> GlobalOpalData;
        public Dictionary<int, int> GlobalOpalCounts;

        public OpalSyncer()
        {
            GlobalOpalData = new Dictionary<int, byte[]>()
            {
                { 0, Enumerable.Repeat((byte)2, 25).ToArray() },
                { 4, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 5, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 6, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 8, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 9, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 10, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 12, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 13, Enumerable.Repeat((byte)2, 300).ToArray() },
                { 14, Enumerable.Repeat((byte)2, 300).ToArray() }
            };
            GlobalOpalCounts = new Dictionary<int, int>()
            {
                { 0, 0 },
                { 4, 0 },
                { 5, 0 },
                { 6, 0 },
                { 8, 0 },
                { 9, 0 },
                { 10, 0 },
                { 12, 0 },
                { 13, 0 },
                { 14, 0 }
            };
        }

        public void HandleServerUpdate(int opal, int level, ushort originalSender)
        {
            if (!GlobalOpalData.Keys.Contains(level)) return;
            if (GlobalOpalData[level][opal] == 5) return;
            GlobalOpalData[level][opal] = 5;
            GlobalOpalCounts[level] = GlobalOpalData[level].Count(i => i == 5);
            SendUpdatedData(opal, level, originalSender);
        }

        public void SendUpdatedData(int index, int level, ushort originalSender)
        {
            Console.WriteLine("sending to clients");
            SyncMessage syncMessage = SyncMessage.Create(index, level, "Opal");
            Server._Server.SendToAll(SyncMessage.Encode(syncMessage), originalSender);
        }

        byte[] ConvertOpals(int level)
        {
            byte[] inputBytes = GlobalOpalData[level];
            byte[] outputBytes = new byte[(int)Math.Ceiling((double)inputBytes.Length / 8)];
            for (int i = 0; i < Math.Ceiling((double)inputBytes.Length / 8); i++)
            {
                byte[] b = inputBytes.Skip(i * 8).Take(8).ToArray();
                if (inputBytes.Skip(i * 8).ToArray().Length % 8 != 0)
                {
                    b = inputBytes.Skip(i * 8).Take(4).ToArray().Concat(new byte[4]).ToArray();
                }
                BitArray bits = new BitArray(8);
                for (int j = 0; j < 8; j++)
                {
                    if (b[j] == 1)
                    {
                        bits[j] = true;
                    }
                }
                bits.CopyTo(outputBytes, i);
            }
            return outputBytes;
        }
    }
}
