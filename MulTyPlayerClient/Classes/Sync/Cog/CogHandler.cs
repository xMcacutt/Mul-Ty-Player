using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            CounterAddress = PointerCalculations.AddOffset(0x265260);
            CounterByteLength = 1;
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveCogSyncer(this);
            SaveSync = new SaveCogSyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();
            foreach (int i in Client.HLevel.MainStages) GlobalObjectData.Add(i, new byte[ObjectAmount]);
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return previousState < 3 && currentState > 3;
        }

        public  override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x270424), new int[]{ 0x0 });
        }
    }
}
