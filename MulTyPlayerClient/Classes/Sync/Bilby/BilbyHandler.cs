using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class BilbyHandler : SyncObjectHandler
    {
        public BilbyHandler()
        {
            Name = "Bilby";
            WriteState = 0;
            CheckState = 0;
            ObjectAmount = 5;
            CounterAddress = PointerCalculations.AddOffset(0x2651AC);
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveTESyncer();
            SaveSync = new SaveTESyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();
            foreach (int i in Program.HLevel.MainStages) GlobalObjectData.Add(i, new byte[ObjectAmount]);
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState == 1 && currentState != 1);
        }

        public override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x2CEB8378), new int[] { 0x0, 0x0 });
        }
    }
}
