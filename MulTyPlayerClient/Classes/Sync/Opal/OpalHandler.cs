using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace MulTyPlayerClient
{
    internal class OpalHandler : SyncObjectHandler
    {
        static LevelHandler HLevel => Program.HLevel;

        public int[] _crateOpalsPerLevel = { 0, 0, 0, 0, 170, 102, 119, 0, 120, 60, 0, 0, 30, 170, 215 };
        
        const int GEM_PTR_LIST_BASE_ADDRESS = 0x28AB7C;
        public int NonCrateOpalsAddress;
        public int RainbowScaleAddress;
        public int CrateOpalsAddress;
        public int B3OpalsAddress;

        public OpalHandler()
        {
            Name = "Opal";
            WriteState = 3;
            CheckState = 5;
            ObjectAmount = 300;
            LiveSync = new LiveOpalSyncer(this);
            SaveSync = new SaveOpalSyncer(this);
            SetSyncClasses(LiveSync, SaveSync);
        }

        public override void SetMemAddrs()
        {
            int gemPtrListAddress = PointerCalculations.AddOffset(GEM_PTR_LIST_BASE_ADDRESS);
            NonCrateOpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[2]);
            RainbowScaleAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] { 0x20, 0 });
            B3OpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] { 0xB20, 0 });
            CrateOpalsAddress = PointerCalculations.GetPointerAddress(gemPtrListAddress, new int[] { 0x4AC, 0 });
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState < 3 && currentState > 3);
        }
    }
}
