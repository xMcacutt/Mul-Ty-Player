using MulTyPlayerClient;
using Riptide;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace MulTyPlayerClient
{
    internal class LevelHandler
    {
        IntPtr HProcess => ProcessHandler.HProcess;
        static KoalaHandler HKoala => Program.HKoala;
        static SyncHandler HSync => Program.HSync;

        public int CurrentLevelId { get; set; }
        public bool LoadedNewLevelGameplaySetupDone;
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
            if (CurrentLevelId == 9 || CurrentLevelId == 13) ObjectiveCountSet();
            LoadedNewLevelGameplaySetupDone = true;
            LoadedNewLevelNetworkingSetupDone = true;
        }

        public void GetCurrentLevel()
        {
            byte[] buffer = new byte[4];
            int bytesRead = 0;
            ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), PointerCalculations.AddOffset(0x280594), buffer, 4, ref bytesRead);
            CurrentLevelId = BitConverter.ToInt32(buffer, 0);
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
            byte[] buffer = new byte[2];
            int bytesRead = 0;
            int currentCountMax = 16;
            int inc = 1;
            while(currentCountMax != 8)
            {
                int objectiveCounterAddr = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x0028C318), objectiveCountOffsets, 2);
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), objectiveCounterAddr, buffer, 2, ref bytesRead);
                if (BitConverter.ToInt16(buffer, 0) == 16)
                {
                    ProcessHandler.WriteData(objectiveCounterAddr, BitConverter.GetBytes(8));
                    currentCountMax = 8;
                }
                inc++;
            }
        }
    }
}
