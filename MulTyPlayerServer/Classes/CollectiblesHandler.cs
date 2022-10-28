using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class CollectiblesHandler
    {
        public static Dictionary<int, byte[]> GlobalLevelData;

        public CollectiblesHandler()
        {
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

        public static void SendUpdatedData(int levelId, ushort originalSender)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ClientDataUpdate);
            message.AddBytes(GlobalLevelData[levelId]);
            message.AddInt(levelId);
            message.AddString("Collectible");
            Server._Server.SendToAll(message, originalSender);
        }
        
        public static void HandleServerUpdate(byte[] bytes, int levelId, ushort returnClientId)
        {
            byte[] temp = GlobalLevelData[levelId];
            SyncHandler.CompDataByteArrays(bytes, ref temp);
            GlobalLevelData[levelId] = temp;
            SendUpdatedData(levelId, returnClientId);
        }
    }
}
