using System.Collections.Generic;
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
            { "Frame", HFrame = new FrameHandler() }
        };
    }

    public void SetMemAddrs()
    {
        SaveDataBaseAddress = PointerCalculations.GetPointerAddress(0x288730, new[] { 0x10 });
        HAttribute.SetMemAddrs();
        if (Levels.GetLevelData(Client.HLevel.CurrentLevelId).FrameCount != 0)
            HFrame.SetMemAddrs();
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
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqSync);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.ReqSync)]
    private static void HandleSyncReqResponse(Message message)
    {
        if (Client.Relaunching) return;
        var type = message.GetString();
        if (SettingsHandler.SyncSettings[type])
            Client.HSync.SyncObjects[type].Sync(message.GetInt(), message.GetBytes(), message.GetBytes());
    }

    [MessageHandler((ushort)MessageID.ClientDataUpdate)]
    private static void HandleClientDataUpdate(Message message)
    {
        if (Client.Relaunching) return;
        var syncMessage = SyncMessage.Decode(message);
        Client.HSync.SyncObjects[syncMessage.type]
            .HandleClientUpdate(syncMessage.iLive, syncMessage.iSave, syncMessage.level);
    }

    public void SendDataToServer(int iLive, int iSave, int level, string type)
    {
        var syncMessage = SyncMessage.Create(iLive, iSave, level, type);
        Client._client.Send(SyncMessage.Encode(syncMessage));
    }

    [MessageHandler((ushort)MessageID.StopWatch)]
    private static void HandleStopWatchActivate(Message message)
    {
        var level = message.GetInt();
        if (Client.HLevel.CurrentLevelData.Id != level || Client.HGameState.IsAtMainMenuOrLoading()) return;
        Client.HSync.ShowStopwatch();
    }

    public void ShowStopwatch()
    {
        var address = PointerCalculations.GetPointerAddress(0x270420, new[] { 0x68 });
        ProcessHandler.WriteData(address, new byte[] { 0x2 });
    }

    public void ProtectLeaderboard()
    {
        var address = SaveDataBaseAddress + 0xB07;
        ProcessHandler.WriteData(address, new byte[] { 1 }, "Protecting leaderboard");
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
        if (SettingsHandler.DoPortalSyncing) HPortal.CheckObserverChanged();
        if (SettingsHandler.DoCliffsSyncing) HCliffs.CheckObserverChanged();
        if (SettingsHandler.DoFrameSyncing) HFrame.CheckObserverChanged();
        if (SettingsHandler.DoRainbowScaleSyncing && Client.HLevel.CurrentLevelId == Levels.RainbowCliffs.Id)
            HRainbowScale.CheckObserverChanged();
    }
}