using System.Collections.Generic;
using MulTyPlayer;
using MulTyPlayerClient.LevelLock;
using Riptide;

namespace MulTyPlayerClient;

internal class SyncHandler
{
    public static OpalHandler HOpal;
    public static TEHandler HThEg;
    public static CogHandler HCog;
    public static BilbyHandler HBilby;
    public static AttributeHandler HAttribute;
    public static PortalHandler HPortal;
    public static CrateHandler HCrate;
    public static RCHandler HCliffs;
    public static RSHandler HRainbowScale;
    public static FrameHandler HFrame;
    public static InvisiCrateHandler HInvisiCrate;
    public LevelLockHandler HLevelLock;
    public TriggerHandler HTrigger;

    public static int SaveDataBaseAddress;
    public Dictionary<string, SyncObjectHandler> SyncObjects;

    public SyncHandler()
    {
        SyncObjects = new Dictionary<string, SyncObjectHandler>
        {
            { "Opal", HOpal = new OpalHandler() },
            { "TE", HThEg = new TEHandler() },
            { "Cog", HCog = new CogHandler() },
            { "Bilby", HBilby = new BilbyHandler() },
            { "Attribute", HAttribute = new AttributeHandler() },
            { "Portal", HPortal = new PortalHandler() },
            { "Crate", HCrate = new CrateHandler() },
            { "RC", HCliffs = new RCHandler() },
            { "RainbowScale", HRainbowScale = new RSHandler() },
            { "Frame", HFrame = new FrameHandler() },
            { "InvisiCrate", HInvisiCrate = new InvisiCrateHandler() }
        };
        HLevelLock = new LevelLockHandler();
        HTrigger = new TriggerHandler();
    }

    public void SetMemAddrs()
    {
        SaveDataBaseAddress = PointerCalculations.GetPointerAddress(0x288730, new[] { 0x10 });
        HTrigger.SetMemAddrs();
        HAttribute.SetMemAddrs();
        if (Levels.GetLevelData(Client.HLevel.CurrentLevelId).FrameCount != 0)
        {
            HFrame.SetMemAddrs();
            HInvisiCrate.SetMemAddrs();
        }
        if (Client.HLevel.CurrentLevelId == Levels.RainbowCliffs.Id)
        {
            HCliffs.SetMemAddrs();
            HRainbowScale.SetMemAddrs();
            HPortal.SetMemAddrs();
        }
        if (Client.HLevel.CurrentLevelData.IsMainStage)
        {
            HOpal.SetMemAddrs();
            HCrate.SetMemAddrs();
            HThEg.SetMemAddrs();
            HBilby.SetMemAddrs();
            HCog.SetMemAddrs();
        }
    }

    public void SetCurrentData(bool inMainStage, bool hasFrames)
    {
        if (hasFrames)
            HFrame.SetCurrentData();
        if (!inMainStage)
            return;
        HCrate.SetCurrentData();
        HCog.SetCurrentData();
        HBilby.SetCurrentData();
        HThEg.SetCurrentData();
        HOpal.SetCurrentData();
    }

    public void RequestSync()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqCollectibleSync);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.ReqCollectibleSync)]
    private static void HandleSyncReqResponse(Message message)
    {
        if (Client.Relaunching) return;
        var type = message.GetString();
        if (SettingsHandler.SyncSettings[type])
            Client.HSync.SyncObjects[type].Sync(message.GetInt(), message.GetBytes(), message.GetBytes());
    }
    
    [MessageHandler((ushort)MessageID.ResetSync)]
    private static void HandleSyncReset(Message message)
    {
        Logger.Write("Synchronisations have been reset to new game state.");
        Client.HSync = new SyncHandler();
    }

    [MessageHandler((ushort)MessageID.ClientCollectibleDataUpdate)]
    private static void HandleClientDataUpdate(Message message)
    {
        if (Client.Relaunching) return;
        var syncMessage = CollectibleSyncMessage.Decode(message);
        Client.HSync.SyncObjects[syncMessage.type]
            .HandleClientUpdate(syncMessage.iLive, syncMessage.iSave, syncMessage.level);
    }

    public void SendDataToServer(int iLive, int iSave, int level, string type)
    {
        var syncMessage = CollectibleSyncMessage.Create(iLive, iSave, level, type);
        Client._client.Send(CollectibleSyncMessage.Encode(syncMessage));
    }

    public void CheckEnabledObservers()
    {
        //OBSERVERS
        if (SettingsHandler.DoOpalSyncing && Levels.GetLevelData(Client.HLevel.CurrentLevelId).IsMainStage)
        {
            HOpal.CheckObserverChanged();
            HCrate.CheckObserverChanged();
        }
        if (SettingsHandler.DoTESyncing) HThEg.CheckObserverChanged();
        if (SettingsHandler.DoCogSyncing) HCog.CheckObserverChanged();
        if (SettingsHandler.DoBilbySyncing) HBilby.CheckObserverChanged();
        if (SettingsHandler.DoRangSyncing) HAttribute.CheckObserverChanged();
        if (SettingsHandler.DoLevelLock) HLevelLock.CheckEntry();
        else if (SettingsHandler.DoPortalSyncing) HPortal.CheckObserverChanged();
        if (SettingsHandler.DoCliffsSyncing) HCliffs.CheckObserverChanged();
        if (SettingsHandler.DoFrameSyncing && (Levels.GetLevelData(Client.HLevel.CurrentLevelId).FrameCount != 0))
        {
            HFrame.CheckObserverChanged();
            HInvisiCrate.CheckObserverChanged();
        }
        if (SettingsHandler.DoRainbowScaleSyncing && Client.HLevel.CurrentLevelId == Levels.RainbowCliffs.Id)
            HRainbowScale.CheckObserverChanged();
    }
}