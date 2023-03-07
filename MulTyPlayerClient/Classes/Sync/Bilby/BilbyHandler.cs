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
            SaveState = 1;
            ObjectAmount = 5;
            SeparateID = true;
            IDOffset = 0x0;
            CounterAddress = PointerCalculations.AddOffset(0x2651AC);
            CounterByteLength = 1;
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveBilbySyncer(this);
            SaveSync = new SaveBilbySyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();
            foreach (int i in Client.HLevel.MainStages) GlobalObjectData.Add(i, Enumerable.Repeat((byte)1, ObjectAmount).ToArray());
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState != 2 && currentState == 2);
        }

        public override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x27D608), new int[] { 0x0, 0x0 });
        }
    }
}
