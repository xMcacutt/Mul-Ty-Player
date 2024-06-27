using System.Collections.Generic;

namespace MulTyPlayerClient;

internal class OpalHandler : SyncObjectHandler
{
    private const int GEM_PTR_LIST_BASE_ADDRESS = 0x28AB7C;
    public int CrateOpalsAddress;

    public int NonCrateOpalsAddress;
    //public int B3OpalsAddress;

    public OpalHandler()
    {
        Name = "Opal";
        WriteState = 3;
        CheckState = 5;
        SaveState = 1;
        ObjectAmount = 300;
        CounterAddress = 0x26547C;
        CounterAddressStatic = true;
        CounterByteLength = 4;
        PreviousObjectData = new byte[ObjectAmount];
        CurrentObjectData = new byte[ObjectAmount];
        PreviousObserverState = 0;
        ObserverState = 0;
        LiveSync = new LiveOpalSyncer(this);
        SaveSync = new SaveOpalSyncer();
        SetSyncClasses(LiveSync, SaveSync);
        GlobalObjectData = new Dictionary<int, byte[]>();

        foreach (var ld in Levels.MainStages) GlobalObjectData.Add(ld.Id, new byte[ObjectAmount]);
    }

    public override void SetMemAddrs()
    {
        NonCrateOpalsAddress = PointerCalculations.GetPointerAddress(GEM_PTR_LIST_BASE_ADDRESS, new[] { 0x0, 0x0 });
        CrateOpalsAddress = PointerCalculations.GetPointerAddress(GEM_PTR_LIST_BASE_ADDRESS, new[] { 0x4AC, 0x0 });
        ProcessHandler.CheckAddress(NonCrateOpalsAddress, (ushort)(17326560 & 0xFFFF), "Opal base address check");
    }

    public override bool CheckObserverCondition(byte previousState, byte currentState)
    {
        return previousState < 3 && currentState > 3;
    }
}