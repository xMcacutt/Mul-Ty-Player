using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class OpalHandler : SyncObjectHandler
    {
        public int[] CrateOpalsPerLevel = { 0, 0, 0, 0, 170, 102, 119, 0, 120, 60, 0, 0, 30, 170, 215 };
        
        const int GEM_PTR_LIST_BASE_ADDRESS = 0x28AB7C;
        public int NonCrateOpalsAddress;
        public int CrateOpalsAddress;
        public int B3OpalsAddress;

        public OpalHandler()
        {
            Name = "Opal";
            WriteState = 3;
            CheckState = 5;
            ObjectAmount = 300;
            CounterAddress = PointerCalculations.AddOffset(0x26547C);
            CounterByteLength = 4;
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            PreviousObserverState = 0;
            ObserverState = 0;
            LiveSync = new LiveOpalSyncer(this);
            SaveSync = new SaveOpalSyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();
            foreach (int i in Program.HLevel.MainStages) GlobalObjectData.Add(i, new byte[ObjectAmount]);
        }

        public override void SetMemAddrs()
        {
            int gemPtrListAddress = PointerCalculations.AddOffset(GEM_PTR_LIST_BASE_ADDRESS);
            NonCrateOpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] {0x0, 0x0});
            B3OpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] {0x2B0, 0x0});
            CrateOpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] {0x4AC, 0x0});
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState < 3 && currentState > 3);
        }
    }
}
