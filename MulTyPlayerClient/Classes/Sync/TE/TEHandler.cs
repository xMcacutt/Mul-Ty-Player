using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            CounterAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0xD);
            CounterByteLength = 1;
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveTESyncer(this);
            SaveSync = new SaveTESyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();
            foreach (int i in Client.HLevel.MainStages) GlobalObjectData.Add(i, new byte[ObjectAmount]);
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState < 3 && currentState > 3);
        }

        public override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x270280), 0x0);
        }
    }
}
