using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class CrateHandler : SyncObjectHandler
    {
        /*
        public CrateHandler()
        {
            Name = "Crate";
            WriteState = 0;
            CheckState = 0;
            CounterAddress = PointerCalculations.AddOffset(0x11877F20);
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveCrateSyncer(this);
            SetSyncClasses(LiveSync);
        }

        */
        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return previousState < 3 && currentState > 3;
        }

        public override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x270280), 0x0);
        }
    }
}
