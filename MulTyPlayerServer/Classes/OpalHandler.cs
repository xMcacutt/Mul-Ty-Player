using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer.Classes
{
    internal class OpalHandler
    {
        public static Dictionary<int, byte[]> GlobalLevelOpalData;
        public static Dictionary<int, byte[]> GlobalSaveOpalData;
        public static Dictionary<int, int> GlobalOpalCounts;

        public OpalHandler()
        {
            GlobalLevelOpalData = new Dictionary<int, byte[]>()
            {
                { 4, new byte[300] },
                { 5, new byte[300] },
                { 6, new byte[300] },
                { 8, new byte[300] },
                { 9, new byte[300] },
                { 10, new byte[300] },
                { 12, new byte[300] },
                { 13, new byte[300] },
                { 14, new byte[300] }
            };
            GlobalSaveOpalData = new Dictionary<int, byte[]>()
            {
                { 4, new byte[38] },
                { 5, new byte[38] },
                { 6, new byte[38] },
                { 8, new byte[38] },
                { 9, new byte[38] },
                { 10, new byte[38] },
                { 12, new byte[38] },
                { 13, new byte[38] },
                { 14, new byte[38] }
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


        [MessageHandler((ushort)MessageID.OpalCollected)]
        static void HandleOpalCollected(ushort fromClientId, Message message)
        {
            int level = message.GetInt();
            int opal = message.GetInt();
            GlobalLevelOpalData[level][opal] = 1;
            GlobalSaveOpalData[level] = ConvertOpals(level);
            GlobalOpalCounts[level]++;

            Message response = Message.Create(MessageSendMode.reliable, MessageID.OpalCollected);
            message.AddInt(level);
            message.AddInt(opal);
            message.AddBytes(GlobalSaveOpalData[level]);
            message.AddInt(GlobalOpalCounts[level]);
            Server._Server.SendToAll(response, fromClientId);
        }

        static byte[] ConvertOpals(int level)
        {
            byte[] inputBytes = GlobalLevelOpalData[level];
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
