using Riptide;
using System;
using System.Collections.Generic;

namespace MulTyPlayerClient
{
    internal class SyncHandler
    {
        IntPtr HProcess = ProcessHandler.HProcess;
        static LevelHandler HLevel => Program.HLevel;

        public Dictionary<string, SyncObjectHandler> SyncObjects;

        public static OpalHandler HOpal;
        public static TEHandler HThEg;
        public static CogHandler HCog;
        public static BilbyHandler HBilby;
        public static AttributeHandler HAttribute;

        public static int SaveDataBaseAddress => PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x288730), 0x10);

        public SyncHandler()
        {
            SyncObjects = new Dictionary<string, SyncObjectHandler>();
            SyncObjects.Add("Opal", HOpal = new OpalHandler());
            SyncObjects.Add("TE", HThEg = new TEHandler());
            SyncObjects.Add("Cog", HCog = new CogHandler());
            SyncObjects.Add("Bilby", HBilby = new BilbyHandler());
            SyncObjects.Add("Attribute", HAttribute = new AttributeHandler());
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
            Program.HSync.SyncObjects[message.GetString()].Sync(message.GetInt(), message.GetBytes(), message.GetBytes());
        }

        [MessageHandler((ushort)MessageID.ClientDataUpdate)]
        private static void HandleClientDataUpdate(Message message)
        {
            SyncMessage syncMessage = SyncMessage.Decode(message);
            Program.HSync.SyncObjects[syncMessage.type].HandleClientUpdate(syncMessage.iLive, syncMessage.iSave, syncMessage.level);
        }
        public void SendDataToServer(int iLive, int iSave, int level, string type)
        {
            SyncMessage syncMessage = SyncMessage.Create(iLive, iSave, level, type);
            Client._client.Send(SyncMessage.Encode(syncMessage));
        }
    }
}
