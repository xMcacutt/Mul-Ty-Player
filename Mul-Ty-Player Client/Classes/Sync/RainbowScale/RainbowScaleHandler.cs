using System.Collections.Generic;

namespace MulTyPlayerClient;

internal class RSHandler : SyncObjectHandler
{
    private const int GEM_PTR_LIST_BASE_ADDRESS = 0x28AB7C;

    public RSHandler()
    {
        Name = "RainbowScale";
        WriteState = 3;
        CheckState = 5;
        SaveState = 1;
        ObjectAmount = 25;
        CounterAddress = 0x2888B0;
        CounterAddressStatic = true;
        CounterByteLength = 1;
        PreviousObjectData = new byte[ObjectAmount];
        CurrentObjectData = new byte[ObjectAmount];
        PreviousObserverState = 0;
        ObserverState = 0;
        LiveSync = new LiveRSSyncer(this);
        SaveSync = new SaveRSSyncer();
        SetSyncClasses(LiveSync, SaveSync);
        GlobalObjectData = new Dictionary<int, byte[]>
        {
            { Levels.RainbowCliffs.Id, new byte[ObjectAmount] }
        };
    }

    public override void SetMemAddrs()
    {
        LiveObjectAddress = PointerCalculations.GetPointerAddress(GEM_PTR_LIST_BASE_ADDRESS, new[] { 0x0, 0x0 });
        ProcessHandler.CheckAddress(LiveObjectAddress, (short)25056, "RS base address check");
    }

    public override bool CheckObserverCondition(byte previousState, byte currentState)
    {
        //May cause issues if state is 3 for 1 or 0 frames???? hasnt yet, dont worry
        return previousState < 3 && currentState > 3;
    }
}