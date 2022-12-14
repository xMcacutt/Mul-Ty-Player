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
        public static PortalHandler HPortal;
        public static CrateHandler HCrate;
        public static RCHandler HCliffs;

        public static int SaveDataBaseAddress => PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x288730), 0x10);

        public SyncHandler()
        {
            SyncObjects = new Dictionary<string, SyncObjectHandler>
            {
                { "Opal", HOpal = new() },
                { "TE", HThEg = new() },
                { "Cog", HCog = new() },
                { "Bilby", HBilby = new() },
                { "Attribute", HAttribute = new() },
                { "Portal", HPortal = new() },
                { "Crate", HCrate = new() },
                { "RC", HCliffs = new() }
            };
        }

        public void SetMemAddrs()
        {
            foreach(SyncObjectHandler syncObject in SyncObjects.Values)
            {
                syncObject.SetMemAddrs();
            }
        }

        public void SetCurrentData(bool inMainStage)
        {
            if (inMainStage)
            {
                HCrate.SetCurrentData();
                HCog.SetCurrentData();
                HBilby.SetCurrentData();
                HThEg.SetCurrentData();
                HOpal.SetCurrentData();
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
            string type = message.GetString();
            if (SettingsHandler.SyncSettings[type]) 
            { 
                Program.HSync.SyncObjects[type].Sync(message.GetInt(), message.GetBytes(), message.GetBytes());
            }
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

        public void ProtectLeaderboard()
        {
            int address = SaveDataBaseAddress + 0xB07;
            int bytesWritten = 0;
            byte[] bytes = new byte[] { 1 };
            ProcessHandler.WriteProcessMemory((int)HProcess, address, bytes, bytes.Length, ref bytesWritten);
        }
    }
}
