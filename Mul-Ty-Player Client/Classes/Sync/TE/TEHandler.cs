using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

internal class  TEHandler : SyncObjectHandler
{
    public TEHandler()
    {
        Name = "TE";
        WriteState = 5;
        CheckState = 5;
        SaveState = 1;
        ObjectAmount = 8;
        SeparateID = true;
        IDOffset = 0x6C;
        CounterAddress = PointerCalculations.GetPointerAddress(0x00288730, new[] { 0xD });
        CounterAddressStatic = false;
        CounterByteLength = 1;
        PreviousObjectData = new byte[ObjectAmount];
        CurrentObjectData = new byte[ObjectAmount];
        LiveSync = new LiveTESyncer(this);
        SaveSync = new SaveTESyncer();
        SetSyncClasses(LiveSync, SaveSync);
        GlobalObjectData = new Dictionary<int, byte[]>();
        foreach (var ld in Levels.MainStages)
            GlobalObjectData.Add(ld.Id, new byte[ObjectAmount]);
    }

    public override bool CheckObserverCondition(byte previousState, byte currentState)
    {
        return previousState < 3 && currentState > 3;
    }

    public override void SetMemAddrs()
    {
        CounterAddress = PointerCalculations.GetPointerAddress(0x00288730, new[] { 0xD });
        LiveObjectAddress = PointerCalculations.GetPointerAddress(0x270280, new[] { 0x0 });
        ProcessHandler.CheckAddress(LiveObjectAddress, (ushort)(17341304 & 0xFFFF), "TE base address check");
    }
    
    [MessageHandler((ushort)MessageID.StopWatch)]
    private static void HandleStopWatchActivate(Message message)
    {
        var level = message.GetInt();
        if (Client.HLevel.CurrentLevelData.Id != level || Client.HGameState.IsOnMainMenuOrLoading) return;
        SFXPlayer.PlaySound(SFX.TAOpen);
        (Client.HSync.SyncObjects["TE"] as TEHandler)?.ShowStopwatch();
    }

    public void ShowStopwatch()
    {
        var address = PointerCalculations.GetPointerAddress(0x270420, new[] { 0x68 });
        ProcessHandler.WriteData(address, new byte[] { 0x2 });
    }
}