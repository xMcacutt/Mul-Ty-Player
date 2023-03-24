using MulTyPlayerClient;
using Riptide;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class LevelHandler
    {
        static KoalaHandler HKoala => Client.HKoala;
        static SyncHandler HSync => Client.HSync;

        public int CurrentLevelId { get; set; }
        public bool bNewLevelSetup;
        public bool LoadedNewLevelNetworkingSetupDone;
        readonly int[] _objectiveCountOffsetsSnow = { 0x30, 0x54, 0x54, 0x6C };
        readonly int[] _objectiveCountOffsetsStump = { 0x30, 0x34, 0x54, 0x6C };

        public int[] MainStages = { 4, 5, 6, 8, 9, 10, 12, 13, 14 };

        public void DoLevelSetup()
        {
            HSync.SetCurrentData(MainStages.Contains(CurrentLevelId));
            HSync.SetMemAddrs();
            HSync.RequestSync();
            HSync.ProtectLeaderboard();
            HKoala.SetBaseAddress();
            if (CurrentLevelId == 9 || CurrentLevelId == 13) ObjectiveCountSet();
            bNewLevelSetup = true;
            LoadedNewLevelNetworkingSetupDone = true;
        }

        public void GetCurrentLevel()
        {
            CurrentLevelId = BitConverter.ToInt32(ProcessHandler.ReadData(PointerCalculations.AddOffset(0x280594), 4, "Getting current level"), 0);
        }

        public void ObjectiveCountSet() 
        {
            int[] objectiveCountOffsets = null;
            switch (CurrentLevelId)
            {
                case 9:
                    objectiveCountOffsets = _objectiveCountOffsetsSnow;
                    break;
                case 13:
                    objectiveCountOffsets = _objectiveCountOffsetsStump;
                    break;
            }
            int currentCountMax = 16;
            int inc = 1;
            while(currentCountMax != 8)
            {
                int objectiveCounterAddr = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x0028C318), objectiveCountOffsets, 2);
                currentCountMax = BitConverter.ToInt16(ProcessHandler.ReadData(objectiveCounterAddr, 2, "Getting koala objective koala count"), 0);
                if (currentCountMax == 16)
                {
                    ProcessHandler.WriteData(objectiveCounterAddr, BitConverter.GetBytes(8), "Setting koala objective koala count");
                    currentCountMax = 8;
                }
                inc++;
            }
        }
    }
}
