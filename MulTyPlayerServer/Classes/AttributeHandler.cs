using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class AttributeHandler
    {
        public static byte[] GlobalAttributeData;
        public AttributeHandler()
        {
            GlobalAttributeData = new byte[26];
        }

        public static void SendUpdatedData(ushort originalSender)
        {
            Console.WriteLine($"Sending to all except client {originalSender}");
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ClientAttributeDataUpdate);
            message.AddBytes(GlobalAttributeData);
            Server._Server.SendToAll(message, originalSender);
        }

        public static void HandleServerUpdate(byte[] bytes, ushort returnClientId)
        {
            SyncHandler.CompDataByteArrays(bytes, ref GlobalAttributeData);
            SendUpdatedData(returnClientId);
        }
    }
}
