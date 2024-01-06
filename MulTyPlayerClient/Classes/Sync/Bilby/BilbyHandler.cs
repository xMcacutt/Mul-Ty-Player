using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

internal class BilbyHandler : SyncObjectHandler
{
    public BilbyHandler()
    {
        Name = "Bilby";
        WriteState = 0;
        CheckState = 0;
        SaveState = 1;
        ObjectAmount = 5;
        SeparateID = true;
        IDOffset = 0x0;
        CounterAddress = 0x2651AC;
        CounterAddressStatic = true;
        CounterByteLength = 1;
        PreviousObjectData = new byte[] { 1, 1, 1, 1, 1 };
        CurrentObjectData = new byte[] { 1, 1, 1, 1, 1 };
        LiveSync = new LiveBilbySyncer(this);
        SaveSync = new SaveBilbySyncer(this);
        SetSyncClasses(LiveSync, SaveSync);
        GlobalObjectData = new Dictionary<int, byte[]>();

        foreach (var ld in Levels.MainStages)
            GlobalObjectData.Add(ld.Id, Enumerable.Repeat((byte)1, ObjectAmount).ToArray());
    }
    
    public override void Sync(int level, byte[] liveData, byte[] saveData)
    {
        SaveSync.Sync(level, saveData);     
        for (var i = 0; i < ObjectAmount; i++)
            if (liveData[i] == CheckState && GlobalObjectData[level][i] != CheckState)
                GlobalObjectData[level][i] = WriteState;
        if (Client.HLevel.CurrentLevelId != level) return;
        LiveSync.Sync(liveData, ObjectAmount, CheckState);
        PreviousObjectData = liveData;
        CurrentObjectData = liveData;
    }

    public override bool CheckObserverCondition(byte previousState, byte currentState)
    {
        return previousState == 1 && currentState != 1;
    }

    public override void SetMemAddrs()
    {
        LiveObjectAddress = PointerCalculations.GetPointerAddress(0x27D608, new[] { 0x0, 0x0 });
        var testAddr = PointerCalculations.GetPointerAddress(0x27D608, new[] { 0x0, 0x64, 0 });
        ProcessHandler.CheckAddress(testAddr, 1601463137, "Bilby base address check");
    }
}