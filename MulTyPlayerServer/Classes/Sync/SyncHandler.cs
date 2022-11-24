﻿using MulTyPlayerServer.Classes.Sync;
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
        public Dictionary<string, Syncer> Syncers;

        public static OpalSyncer SOpal;
        public static TESyncer SThEg;
        public static CogSyncer SCog;
        public static BilbySyncer SBilby;
        public static AttributeSyncer SAttribute;

        public SyncHandler()
        {
            Syncers = new Dictionary<string, Syncer>();
            Syncers.Add("Opal", SOpal = new OpalSyncer());
            Syncers.Add("TE", SThEg = new TESyncer());
            Syncers.Add("Cog", SCog = new CogSyncer());
            Syncers.Add("Bilby", SBilby = new BilbySyncer());
            Syncers.Add("Attribute", SAttribute = new AttributeSyncer());
        }

        public static void SendResetSyncMessage()
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ResetSync);
            Server._Server.SendToAll(message);
        }

        [MessageHandler((ushort)MessageID.ReqSync)]
        private static void HandleSyncRequest(ushort fromClientId, Message message)
        {
            foreach(Syncer s in Program.HSync.Syncers.Values)
            {
                s.Sync(fromClientId);
            }
        }

        [MessageHandler((ushort)MessageID.ServerDataUpdate)]
        private static void HandleServerDataUpdate(ushort fromClientId, Message message)
        {
            SyncMessage syncMessage = SyncMessage.Decode(message);
            Program.HSync.Syncers[syncMessage.type].HandleServerUpdate(syncMessage.iLive, syncMessage.iSave, syncMessage.level, fromClientId);
        }
    }
}
