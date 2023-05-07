using Riptide;
using MulTyPlayerClient.Networking;
using MulTyPlayerCommon;
using System.Collections.Generic;
using MulTyPlayerClient.Memory;
using MulTyPlayerClient.Settings;

namespace MulTyPlayerClient.Sync
{
    internal class SyncHandler
    {
        public Dictionary<string, SyncObjectHandler> SyncObjects;

        public static OpalHandler HOpal;
        public static TEHandler HThEg;
        public static CogHandler HCog;
        public static BilbyHandler HBilby;
        public static AttributeHandler HAttribute;
        public static PortalHandler HPortal;
        public static CrateHandler HCrate;
        public static RCHandler HCliffs;
        public static RSHandler HRainbowScale;

        public static int SaveDataBaseAddress;

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
                { "RC", HCliffs = new() },
                { "RainbowScale", HRainbowScale = new() },
            };
        }

        public void SetMemAddrs()
        {
            foreach (SyncObjectHandler syncObject in SyncObjects.Values)
            {
                SaveDataBaseAddress = Addresses.GetPointerAddress(0x288730, new int[] { 0x10 });
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
            ConnectionService.Client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReqSync)]
        private static void HandleSyncReqResponse(Message message)
        {
            if (Replication.Relaunching) return;
            string type = message.GetString();
            if (SettingsHandler.SyncSettings[type])
            {
                Replication.HSync.SyncObjects[type].Sync(message.GetInt(), message.GetBytes(), message.GetBytes());
            }
        }

        [MessageHandler((ushort)MessageID.ClientDataUpdate)]
        private static void HandleClientDataUpdate(Message message)
        {
            if (Replication.Relaunching) return;
            SyncMessage syncMessage = SyncMessage.Decode(message);
            Replication.HSync.SyncObjects[syncMessage.type].HandleClientUpdate(syncMessage.iLive, syncMessage.iSave, syncMessage.level);
        }
        public void SendDataToServer(int iLive, int iSave, int level, string type)
        {
            SyncMessage syncMessage = SyncMessage.Create(iLive, iSave, level, type);
            ConnectionService.Client.Send(SyncMessage.Encode(syncMessage));
        }

        public void ProtectLeaderboard()
        {
            int address = SaveDataBaseAddress + 0xB07;
            ProcessHandler.WriteData(address, new byte[] { 1 }, "Protecting leaderboard");
        }

        public void CheckEnabledObservers()
        {
            //OBSERVERS
            if (SettingsHandler.DoOpalSyncing && Levels.GetLevelData(LevelHandler.CurrentLevelId).IsMainStage)
            {
                HOpal.CheckObserverChanged();
                HCrate.CheckObserverChanged();
            }
            if (SettingsHandler.DoTESyncing) HThEg.CheckObserverChanged();
            if (SettingsHandler.DoCogSyncing) HCog.CheckObserverChanged();
            if (SettingsHandler.DoBilbySyncing) HBilby.CheckObserverChanged();
            if (SettingsHandler.DoRangSyncing) HAttribute.CheckObserverChanged();
            if (SettingsHandler.DoPortalSyncing) HPortal.CheckObserverChanged();
            if (SettingsHandler.DoCliffsSyncing) HCliffs.CheckObserverChanged();

            if (SettingsHandler.DoRainbowScaleSyncing && LevelHandler.CurrentLevelId == Levels.RainbowCliffs.Id) HRainbowScale.CheckObserverChanged();
        }
    }
}
