using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class SyncHandler
    {
        public static int[] MainStages = { 4, 5, 6, 8, 9, 10, 12, 13, 14 };
        public SyncHandler()
        {
            CollectiblesHandler collectiblesHandler = new CollectiblesHandler();
            AttributeHandler attributeHandler = new AttributeHandler();
            OpalHandler opalHandler = new OpalHandler();
        }

        public static void CompDataByteArrays(byte[] localPlayerArray, ref byte[] globalDataArray)
        {
            for (int i = 0; i < localPlayerArray.Length; i++)
            {
                if (localPlayerArray[i] == 1 && globalDataArray[i] == 0)
                {
                    globalDataArray[i] = 1;
                }
            }
        }

        [MessageHandler((ushort)MessageID.ServerDataUpdate)]
        private static void HandleServerDataUpdate(ushort fromClientId, Message message)
        {
            byte[] playerDataArray = message.GetBytes();
            int levelId = message.GetInt();
            string dataType = message.GetString();
            switch (dataType)
            {
                case "Attribute": AttributeHandler.HandleServerUpdate(playerDataArray, fromClientId); break;
                case "Collectible": CollectiblesHandler.HandleServerUpdate(playerDataArray, levelId, fromClientId); break;
                case "Opal": OpalHandler.HandleServerUpdate(BitConverter.ToInt16(playerDataArray, 0), levelId, fromClientId); break;
                case "Portal": break;
                case "Julius": break;
                case "Scale": break;
                case "Cliffs": break;
                default: break;
            }
        }

        public static void SendResetSyncMessage()
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ResetSync);
            Server._Server.SendToAll(message);
        }

        [MessageHandler((ushort)MessageID.ReqSync)]
        public static void SyncRequest(ushort fromClientId, Message message)
        {
            int level = message.GetInt();
            Message sync = Message.Create(MessageSendMode.reliable, MessageID.ReqSync);
            
            foreach (byte[] bytes in CollectiblesHandler.GlobalLevelData.Values)
            {
                sync.AddBytes(bytes);
            }

            if (SettingsHandler.DoSyncOpals && MainStages.Contains(level))
            {
                sync.AddBytes(OpalHandler.GlobalOpalData[level], true, true);
            }

            if (SettingsHandler.DoSyncRangs)
            {
                sync.AddBytes(AttributeHandler.GlobalAttributeData);
            }

            Server._Server.Send(sync, fromClientId);
        }

    }
}
