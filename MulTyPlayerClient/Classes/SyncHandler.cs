using MulTyPlayerClient;
using MulTyPlayerClient.Classes;
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
        static OpalHandler HOpal => Program.HOpal;

        public static byte[] _lastReceivedServerData;

        public void UpdateServerData(int level, byte[] data, string type)
        {
            if(Enumerable.SequenceEqual(data, _lastReceivedServerData)) { return; }
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ServerDataUpdate);
            message.AddBytes(data);
            message.AddInt(level);
            message.AddString(type);
            Client._client.Send(message);
        }

        public void RequestSync()
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ReqSync);
            message.AddInt(Program.HLevel.CurrentLevelId);
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

            HOpal.CurrentOpalData = message.GetBytes();
            HOpal.WriteCurrentOpalsSync();

            foreach (int i in tempints)
            {
                HOpal.LevelOpalData[i] = message.GetBytes();
                ProcessHandler.WriteData(HOpal.levelOpalDataAddress + (0x70 * (i - 4)), HOpal.LevelOpalData[i]);
            }

            HAttribute.AttributeData = message.GetBytes();
            ProcessHandler.WriteData(HAttribute.AttributeDataBaseAddress, HAttribute.AttributeData);
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
