using MulTyPlayerClient;
using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SyncHandler
    {
        IntPtr HProcess = ProcessHandler.HProcess;
        static AttributeHandler HAttribute => Program.HAttribute;
        static CollectiblesHandler HCollectibles => Program.HCollectibles;

        public void UpdateServerData(int level, byte[] data, string type)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ServerDataUpdate);
            message.AddBytes(data);
            message.AddInt(level);
            message.AddString(type);
            Client._client.Send(message);
        }

        public void RequestSync()
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ReqSync);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReqSync)]
        public static void HandleSyncRequestResponse(Message message)
        {
            int[] tempints = HCollectibles.LevelData.Keys.ToArray();
            foreach (int i in tempints)
            {
                HCollectibles.LevelData[i] = message.GetBytes();
                ProcessHandler.WriteData(HCollectibles.LevelDataStartAddress + (0x70 * (i - 4)), HCollectibles.LevelData[i]);
            }
            HAttribute.AttributeData = message.GetBytes();
            ProcessHandler.WriteData(HAttribute.AttributeDataBaseAddress, HAttribute.AttributeData);
        }

        [MessageHandler((ushort)MessageID.ClientAttributeDataUpdate)]
        public static void UpdateClientWithAttr(Message message)
        {
            HAttribute.AttributeData = message.GetBytes();
            ProcessHandler.WriteData(HAttribute.AttributeDataBaseAddress, HAttribute.AttributeData);
        }

        [MessageHandler((ushort)MessageID.ClientLevelDataUpdate)]
        public static void UpdateClientWithLevelData(Message message)
        {
            int level = message.GetInt();
            HCollectibles.LevelData[level] = message.GetBytes();
            bool[] doSync = message.GetBools();
            if (doSync[0]) { ProcessHandler.WriteData(HCollectibles.LevelDataStartAddress + (0x70 * (level - 4)), HCollectibles.LevelData[level].Take(8).ToArray()); }
            if (doSync[1]) { ProcessHandler.WriteData(HCollectibles.LevelDataStartAddress + (0x70 * (level - 4)) + 0x8, HCollectibles.LevelData[level].Skip(8).Take(10).ToArray()); }
            if (doSync[2]) { ProcessHandler.WriteData(HCollectibles.LevelDataStartAddress + (0x70 * (level - 4)) + 0x12, HCollectibles.LevelData[level].Skip(18).Take(5).ToArray()); }
        }

        [MessageHandler((ushort)MessageID.ResetSync)]
        public static void ResetSync(Message message)
        {
            int[] tempints = HCollectibles.LevelData.Keys.ToArray();
            foreach (int i in tempints)
            {
                HCollectibles.LevelData[i] = new byte[23];
            }
            HAttribute.AttributeData = new byte[26];
            HAttribute.AttributeData[4] = 1;
            HAttribute.PreviousAttributeData = new byte[26];
            HCollectibles.CollectibleCounts = new int[3];
            HCollectibles.PreviousCollectibleCounts = new int[3];
        }

        public void ProtectLeaderboard()
        {
            int address = HAttribute.AttributeDataBaseAddress + 0x5F;
            int bytesWritten = 0;
            byte[] bytes = BitConverter.GetBytes(100000000);
            ProcessHandler.WriteProcessMemory((int)HProcess, address, bytes, bytes.Length, ref bytesWritten);
        }
    }
}
