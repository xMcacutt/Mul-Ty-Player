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
        public static byte[] GlobalAttributeData;

        public CollectiblesHandler()
        {
            GlobalLevelData = new Dictionary<int, byte[]>();

            GlobalAttributeData = new byte[26];
            
            GlobalLevelData.Add(4, new byte[23]);
            GlobalLevelData.Add(5, new byte[23]);
            GlobalLevelData.Add(6, new byte[23]);
            
            GlobalLevelData.Add(8, new byte[23]);
            GlobalLevelData.Add(9, new byte[23]);
            GlobalLevelData.Add(10, new byte[23]);

            GlobalLevelData.Add(12, new byte[23]);
            GlobalLevelData.Add(13, new byte[23]);
            GlobalLevelData.Add(14, new byte[23]);
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
            int currentLevel = message.GetInt();
            string dataType = message.GetString();
            if (dataType == "Attribute")
            {
                CompDataArrays(playerDataArray, ref GlobalAttributeData);
                SendUpdatedAttributeData(fromClientId);
            }
            if (dataType == "Collectible")
            {
                byte[] tempArray = GlobalLevelData[currentLevel];
                CompDataArrays(playerDataArray, ref tempArray);
                GlobalLevelData[currentLevel] = tempArray;
                SendUpdatedLevelData(currentLevel, fromClientId);
            }
        } 

        public static void SendUpdatedLevelData(int levelId, ushort originalSender)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ClientLevelDataUpdate);
            message.AddBytes(GlobalLevelData[levelId]);
            Program.Server.SendToAll(message, originalSender);
        }

        public static void SendUpdatedAttributeData(ushort originalSender)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ClientAttributeDataUpdate);
            message.AddBytes(GlobalAttributeData);
            Program.Server.SendToAll(message, originalSender);
        }
    }
}
