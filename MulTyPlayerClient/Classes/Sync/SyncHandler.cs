using Riptide;
using System;

namespace MulTyPlayerClient
{
    internal class SyncHandler
    {
        IntPtr HProcess = ProcessHandler.HProcess;
        static LevelHandler HLevel => Program.HLevel;

        public static OpalHandler HOpal;

        public SyncHandler()
        {
            HOpal = new OpalHandler();
        }

        public void SendDataToServer(int index, int level, string type)
        {
            Console.WriteLine("sending to server");
            SyncMessage syncMessage = SyncMessage.Create(index, level, type);
            Client._client.Send(SyncMessage.Encode(syncMessage));
        }

        public void SetMemAddrs()
        {
            HOpal.SetMemAddrs();
        }

        [MessageHandler((ushort)MessageID.ClientDataUpdate)]
        private static void HandleClientDataUpdate(Message message)
        {
            Console.WriteLine("handling data from server");
            SyncMessage syncMessage = SyncMessage.Decode(message);
            Console.WriteLine($"opal number: {syncMessage.index} collected");
            switch (syncMessage.type)
            {
                case "Opal": HOpal.HandleClientUpdate(syncMessage.index, syncMessage.level); break;
                default: { break; }
            }
        }
    }
}
