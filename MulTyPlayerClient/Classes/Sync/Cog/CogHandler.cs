using System.Collections.Generic;

namespace MulTyPlayerClient
{
    internal class CogHandler : SyncObjectHandler
    {
        public CogHandler()
        {
            Name = "Cog";
            WriteState = 5;
            CheckState = 5;
            SaveState = 1;
            ObjectAmount = 10;
            SeparateID = true;
            IDOffset = 0x6C;
            CounterAddress = 0x265260;
            CounterAddressStatic = true;
            CounterByteLength = 1;
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveCogSyncer(this);
            SaveSync = new SaveCogSyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();

            foreach (LevelData ld in Levels.MainStages)
                GlobalObjectData.Add(ld.Id, new byte[ObjectAmount]);
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return previousState < 3 && currentState > 3;
        }

        public  override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(0x270424, new int[]{ 0x0 });
            ProcessHandler.CheckAddress(LiveObjectAddress, 17341304, "Cog base address check");
        }
    }
}
