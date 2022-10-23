using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerServerCLI
{
    internal class CollectiblesHandler
    {
        public static Dictionary<int, byte[]> GlobalLevelData;
        public Dictionary<int, int> CollectibleLevelMultipliers;
        public static byte[] GlobalAttributeData;

        public CollectiblesHandler()
        {
            GlobalAttributeData = new byte[26];

            GlobalLevelData = new Dictionary<int, byte[]>
            {
                { 4, new byte[23] },
                { 5, new byte[23] },
                { 6, new byte[23] },

                { 8, new byte[23] },
                { 9, new byte[23] },
                { 10, new byte[23] },

                { 12, new byte[23] },
                { 13, new byte[23] },
                { 14, new byte[23] }
            };

        }

        public static void CompDataArrays(byte[] localPlayerArray, ref byte[] globalDataArray)
        {
            for (int i = 0; i < localPlayerArray.Length; i++)
            {
                if (localPlayerArray[i] == 1 && globalDataArray[i] == 0)
                {
                    globalDataArray[i] = 1;
                }
            }
        }

        /*MESSAGE HANDLING*/

        [MessageHandler((ushort)MessageID.ServerDataUpdate)]
        private static void HandleServerDataUpdate(ushort fromClientId, Message message)
        {
            byte[] playerDataArray = message.GetBytes();
            int levelId = message.GetInt();
            string dataType = message.GetString();
            if (dataType == "Attribute" && SettingsHandler.DoSyncRangs)
            {
                CompDataArrays(playerDataArray, ref GlobalAttributeData);
                SendUpdatedAttributeData(fromClientId);
            }
            if (dataType == "Collectible")
            {
                byte[] tempArray = GlobalLevelData[levelId];
                CompDataArrays(playerDataArray, ref tempArray);
                GlobalLevelData[levelId] = tempArray;
                SendUpdatedLevelData(levelId, fromClientId, SettingsHandler.DoSyncTEs, SettingsHandler.DoSyncCogs, SettingsHandler.DoSyncBilbies);
            }
        }

        public static void SendResetSyncMessage()
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ResetSync);
            Program.Server.SendToAll(message);
        }

        public static void SendUpdatedLevelData(int levelId, ushort originalSender, bool doSyncTEs, bool doSyncCogs, bool doSyncBilbies)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ClientLevelDataUpdate);
            message.AddInt(levelId);
            message.AddBytes(GlobalLevelData[levelId]);
            message.AddBools(new bool[] { doSyncTEs, doSyncCogs, doSyncBilbies });
            Program.Server.SendToAll(message, originalSender);
        }

        public static void SendUpdatedAttributeData(ushort originalSender)
        {
            Console.WriteLine($"Sending to all except client {originalSender}");
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ClientAttributeDataUpdate);
            message.AddBytes(GlobalAttributeData);
            Program.Server.SendToAll(message, originalSender);
        }

        [MessageHandler((ushort)MessageID.ReqSync)]
        public static void SyncRequest(ushort fromClientId, Message message)
        {
            Message sync = Message.Create(MessageSendMode.reliable, MessageID.ReqSync);
            //ADD LEVEL COLLECTIBLE DATA FOR EACH LEVEL
            foreach(byte[] bytes in GlobalLevelData.Values)
            {
                sync.AddBytes(bytes);
            }
            sync.AddBytes(GlobalAttributeData);
            Program.Server.Send(sync, fromClientId);
        }

    }
}
