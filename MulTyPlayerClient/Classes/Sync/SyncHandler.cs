using Riptide;
using System;
using System.Collections.Generic;

namespace MulTyPlayerClient
{
    internal class SyncHandler
    {
        IntPtr HProcess = ProcessHandler.HProcess;
        static LevelHandler HLevel => Program.HLevel;

        public static Dictionary<string, SyncObjectHandler> SyncObjects;

        public static OpalHandler HOpal;
        public static TEHandler HThEg;
        public static CogHandler HCog;
        public static BilbyHandler HBilby;

        public static int SaveDataBaseAddress => PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x288730), 0x0);

        public SyncHandler()
        {
            SyncObjects = new Dictionary<string, SyncObjectHandler>();
            SyncObjects.Add("Opal", HOpal = new OpalHandler());
            SyncObjects.Add("TE", HThEg = new TEHandler());
            SyncObjects.Add("Cog", HCog = new CogHandler());
            SyncObjects.Add("Bilby", HBilby = new BilbyHandler());
        }

        public void SetMemAddrs()
        {
            foreach(SyncObjectHandler syncObject in SyncObjects.Values)
            {
                syncObject.SetMemAddrs();
            }
        }

        public void RequestSync()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ReqSync);
            Client._client.Send(message);
        }
        [MessageHandler((ushort)MessageID.ReqSync)]
        private static void HandleSyncReqResponse(Message message)
        {
            SyncObjects[message.GetString()].Sync(message.GetInt(), message.GetBytes());
        }

        [MessageHandler((ushort)MessageID.ClientDataUpdate)]
        private static void HandleClientDataUpdate(Message message)
        {
            SyncMessage syncMessage = SyncMessage.Decode(message);
            SyncObjects[syncMessage.type].HandleClientUpdate(syncMessage.index, syncMessage.level);
        }
        public void SendDataToServer(int index, int level, string type)
        {
            SyncMessage syncMessage = SyncMessage.Create(index, level, type);
            Client._client.Send(SyncMessage.Encode(syncMessage));
        }
    }
}
