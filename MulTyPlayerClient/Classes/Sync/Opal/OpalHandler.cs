using System.Collections.Generic;

namespace MulTyPlayerClient
{
    internal class OpalHandler : SyncObjectHandler
    {
        const int GEM_PTR_LIST_BASE_ADDRESS = 0x28AB7C;
        public int NonCrateOpalsAddress;
        public int CrateOpalsAddress;
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

            foreach (LevelData ld in Levels.MainStages)
            {
                GlobalObjectData.Add(ld.Id, new byte[ObjectAmount]);
            }
        }

        public  override void SetMemAddrs()
        {
            int gemPtrListAddress = GEM_PTR_LIST_BASE_ADDRESS;
            NonCrateOpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] {0x0, 0x0});
            //B3OpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] {0x2B0, 0x0});
            CrateOpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] {0x4AC, 0x0});
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState < 3 && currentState > 3);
        }
    }
}
