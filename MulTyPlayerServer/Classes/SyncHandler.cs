﻿using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class SyncHandler
    {
        
        public SyncHandler()
        {
            CollectiblesHandler collectiblesHandler = new CollectiblesHandler();
            AttributeHandler attributeHandler = new AttributeHandler();
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

        /*MESSAGE HANDLING*/

        [MessageHandler((ushort)MessageID.ServerDataUpdate)]
        private static void HandleServerDataUpdate(ushort fromClientId, Message message)
        {
            byte[] playerDataArray = message.GetBytes();
            int levelId = message.GetInt();
            string dataType = message.GetString();
            switch (dataType)
            {
                case "Attribute": if(SettingsHandler.DoSyncRangs) { AttributeHandler.HandleServerUpdate(playerDataArray, fromClientId); } break;
                case "Collectible": { CollectiblesHandler.HandleServerUpdate(playerDataArray, levelId, fromClientId); } break;
                case "Opal": { break; }
                case "Portal": { break; }
                case "Julius": { break; }
                case "Scale": { break; }
                case "Cliffs": { break; }
                default: { break; }
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
            Message sync = Message.Create(MessageSendMode.reliable, MessageID.ReqSync);
            //ADD LEVEL COLLECTIBLE DATA FOR EACH LEVEL
            foreach (byte[] bytes in CollectiblesHandler.GlobalLevelData.Values)
            {
                sync.AddBytes(bytes);
            }
            sync.AddBytes(AttributeHandler.GlobalAttributeData);
            Server._Server.Send(sync, fromClientId);
        }

    }
}
