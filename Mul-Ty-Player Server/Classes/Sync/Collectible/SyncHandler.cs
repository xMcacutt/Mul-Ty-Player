using System.Collections.Generic;
using MulTyPlayer;
using MulTyPlayerServer.Classes.Sync;
using Riptide;

namespace MulTyPlayerServer;

internal class SyncHandler
{
    public static int[] MainStages = { 4, 5, 6, 8, 9, 10, 12, 13, 14 };

    public static OpalSyncer SOpal;
    public static TESyncer SThEg;
    public static CogSyncer SCog;
    public static BilbySyncer SBilby;
    public static AttributeSyncer SAttribute;
    public static PortalSyncer SPortal;
    public static CrateSyncer SCrate;
    public static RCSyncer SCliffs;
    public static RainbowScaleSyncer SRainbowScale;
    public static FrameSyncer SFrame;
    public static InvisiCrateSyncer SInvisiCrate;
    public Dictionary<string, Syncer> Syncers;

    public SyncHandler()
    {
        Syncers = new Dictionary<string, Syncer>
        {
            { "Opal", SOpal = new OpalSyncer() },
            { "TE", SThEg = new TESyncer() },
            { "Cog", SCog = new CogSyncer() },
            { "Bilby", SBilby = new BilbySyncer() },
            { "Attribute", SAttribute = new AttributeSyncer() },
            { "Portal", SPortal = new PortalSyncer() },
            { "Crate", SCrate = new CrateSyncer() },
            { "RC", SCliffs = new RCSyncer() },
            { "RainbowScale", SRainbowScale = new RainbowScaleSyncer() },
            { "Frame", SFrame = new FrameSyncer() },
            { "InvisiCrate", SInvisiCrate = new InvisiCrateSyncer() }
        };
        LevelLockHandler.ActiveLevel = 0;
        LevelLockHandler.CompletedLevels = new List<int>();
    }

    public static void SendResetSyncMessage()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ResetSync);
        Server._Server.SendToAll(message);
    }

    [MessageHandler((ushort)MessageID.ReqCollectibleSync)]
    private static void HandleSyncRequest(ushort fromClientId, Message message)
    {
        foreach (var s in Program.HSync.Syncers.Keys)
            if (SettingsHandler.SyncSettings[s])
                Program.HSync.Syncers[s].Sync(fromClientId);
    }

    [MessageHandler((ushort)MessageID.ServerCollectibleDataUpdate)]
    private static void HandleServerDataUpdate(ushort fromClientId, Message message)
    {
        var syncMessage = SyncMessage.Decode(message);
        Program.HSync.Syncers[syncMessage.type]
        //Console.WriteLine($"{syncMessage.type} {syncMessage.iLive} collected in level {syncMessage.level} by client {fromClientId}.");
            .HandleServerUpdate(syncMessage.iLive, syncMessage.iSave, syncMessage.level, fromClientId);
    }
}

