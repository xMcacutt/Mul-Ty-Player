using Riptide;
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

        public static OpalSyncer SOpal;
        public static int i = 0;

        public SyncHandler()
        {
            SOpal = new OpalSyncer();
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
            int index = message.GetInt();
            int level = message.GetInt();
            string dataType = message.GetString();
            switch (dataType)
            {
                case "Opal": SOpal.HandleServerUpdate(index, level, fromClientId); break;
                default: break;
            }
        }

        /*
        public static void SendResetSyncMessage()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ResetSync);
            Server._Server.SendToAll(message);
        }

        [MessageHandler((ushort)MessageID.ReqSync)]
        public static void SyncRequest(ushort fromClientId, Message message)
        {
            int level = message.GetInt();
            Message sync = Message.Create(MessageSendMode.Reliable, MessageID.ReqSync);

            sync.AddBytes(SOpal.GlobalOpalData[level]);
            Server._Server.Send(sync, fromClientId);
        }
        */
    }
}
