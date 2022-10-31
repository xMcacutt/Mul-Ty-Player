using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class OpalHandler
    {
        public static Dictionary<int, byte[]> GlobalOpalData;
        public static Dictionary<int, int> GlobalOpalCounts;

        public OpalHandler()
        {
            GlobalOpalData = new Dictionary<int, byte[]>()
            {
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

        public static void HandleServerUpdate(int opal, int level, ushort fromClientId)
        {
            if (!GlobalOpalData.Keys.Contains(level)) { return; }
            GlobalOpalData[level][opal] = 5;
            GlobalOpalCounts[level] = GlobalOpalData[level].Count(i => i == 5);

            SendUpdatedData(opal, level, fromClientId);
        }

        public static void SendUpdatedData(int opal, int level, ushort fromClientId)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ClientDataUpdate);
            message.AddBytes(BitConverter.GetBytes(opal));
            message.AddInt(level);
            message.AddString("Opal");
            message.AddInt(GlobalOpalCounts[level]);
            Server._Server.SendToAll(message);
        }

        static byte[] ConvertOpals(int level)
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
