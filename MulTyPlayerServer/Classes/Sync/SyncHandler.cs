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
            Console.WriteLine("Handling data from client");
            SyncMessage syncMessage = SyncMessage.Decode(message);
            switch (syncMessage.type)
            {
                case "Opal": SOpal.HandleServerUpdate(syncMessage.index, syncMessage.level, fromClientId); break;
                default: break;
            }
        }
    }
}
