using System.Collections.Generic;

namespace MulTyPlayerClient
{
    internal class TEHandler : SyncObjectHandler
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
            CounterAddress = PointerCalculations.GetPointerAddress(0x00288730, new int[] { 0xD });
            CounterAddressStatic = false;
            CounterByteLength = 1;
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveTESyncer(this);
            SaveSync = new SaveTESyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();
            foreach (LevelData ld in Levels.MainStages)
                GlobalObjectData.Add(ld.Id, new byte[ObjectAmount]);
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState < 3 && currentState > 3);
        }

        public  override void SetMemAddrs()
        {
            CounterAddress = PointerCalculations.GetPointerAddress(0x00288730, new int[] { 0xD });
            LiveObjectAddress = PointerCalculations.GetPointerAddress(0x270280, new int[] { 0x0 });
            ProcessHandler.CheckAddress(LiveObjectAddress, 17341304, "TE base address check");
        }
    }
}
