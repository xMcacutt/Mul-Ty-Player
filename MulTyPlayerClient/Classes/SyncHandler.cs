using MulTyPlayerClient;
using MulTyPlayerClient.Classes;
using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Cache;
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

        public SyncHandler()
        {
            _lastReceivedServerData = new byte[1];
        }

        public void UpdateServerData(int level, byte[] data, string type)
        {
            if(Enumerable.SequenceEqual(data, _lastReceivedServerData)) { return; }
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ServerDataUpdate);
            message.AddBytes(data);
            message.AddInt(level);
            message.AddString(type);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ClientDataUpdate)]
        private static void HandleClientDataUpdate(Message message)
        {
            byte[] data = message.GetBytes();
            _lastReceivedServerData = data;
            int level = message.GetInt();
            string dataType = message.GetString();
            switch (dataType)
            {
                case "Attribute": HAttribute.HandleClientUpdate(data); break;
                case "Collectible": HCollectibles.HandleClientUpdate(data, level); break;
                case "Opal": HOpal.HandleClientUpdate(data[0], level, message.GetInt()); break;
                case "Portal": { break; }
                case "Julius": { break; }
                case "Scale": { break; }
                case "Cliffs": { break; }
                default: { break; }
            }
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
            OpalHandler.SetMemAddrs();

            foreach (int i in Program.HLevel.MainStages)
            {
                HCollectibles.LevelData[i] = message.GetBytes();
                if (SettingsHandler.DoTESyncing)
                {
                    ProcessHandler.WriteData(HCollectibles.LevelDataStartAddress + (0x70 * (i - 4)), HCollectibles.LevelData[i].Take(8).ToArray());
                }
                if (SettingsHandler.DoTESyncing)
                {
                    ProcessHandler.WriteData(HCollectibles.LevelDataStartAddress + (0x70 * (i - 4)) + 8, HCollectibles.LevelData[i].Skip(8).Take(10).ToArray());
                }
                if (SettingsHandler.DoTESyncing)
                {
                    ProcessHandler.WriteData(HCollectibles.LevelDataStartAddress + (0x70 * (i - 4)) + 18, HCollectibles.LevelData[i].Skip(18).Take(5).ToArray());
                }
            }

            if (SettingsHandler.DoOpalSyncing && Program.HLevel.MainStages.Contains(Program.HLevel.CurrentLevelId)) 
            {
                byte[] bytes = message.GetBytes(true);
                HOpal.CurrentOpalData = bytes;
                HOpal.PreviousOpalData = bytes;
                HOpal.WriteCurrentOpalsSync(bytes);
            }

            if (SettingsHandler.DoRangSyncing)
            {
                HAttribute.AttributeData = message.GetBytes();
                ProcessHandler.WriteData(HAttribute.AttributeDataBaseAddress, HAttribute.AttributeData);
            }
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
            HOpal.CurrentOpalData = new byte[300];
            HOpal.PreviousOpalData = new byte[300];
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
