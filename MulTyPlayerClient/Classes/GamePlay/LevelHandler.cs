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

        public async Task DoLevelSetup()
        {
            HSync.SetCurrentData(MainStages.Contains(CurrentLevelId));
            HSync.SetMemAddrs();
            HSync.RequestSync();
            await HSync.ProtectLeaderboard();
            await HKoala.SetBaseAddress();
            if (CurrentLevelId == 9 || CurrentLevelId == 13) await ObjectiveCountSet();
            bNewLevelSetup = true;
            LoadedNewLevelNetworkingSetupDone = true;
        }

        public async Task GetCurrentLevel()
        {
            CurrentLevelId = BitConverter.ToInt32(await ProcessHandler.ReadDataAsync(PointerCalculations.AddOffset(0x280594), 4), 0);
        }

        public async Task ObjectiveCountSet() 
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
                int objectiveCounterAddr = await PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x0028C318), objectiveCountOffsets, 2);
                currentCountMax = BitConverter.ToInt16(await ProcessHandler.ReadDataAsync(objectiveCounterAddr, 2), 0);
                if (currentCountMax == 16)
                {
                    await ProcessHandler.WriteDataAsync(objectiveCounterAddr, BitConverter.GetBytes(8));
                    currentCountMax = 8;
                }
                inc++;
            }
        }
    }
}
