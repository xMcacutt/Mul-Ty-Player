using MulTyPlayerClient.GUI;
using Riptide;
using System;
using System.Linq;

namespace MulTyPlayerClient
{
    internal class LevelHandler
    {
        static KoalaHandler HKoala => Client.HKoala;
        static SyncHandler HSync => Client.HSync;

        public int CurrentLevelId { get; set; }
        public bool bNewLevelSetup;

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
        }

        public void GetCurrentLevel()
        {
            if (BasicIoC.MainGUIViewModel.IsOnMenu) return;
            ProcessHandler.TryRead(0x280594, out int result, true);
            if (BasicIoC.MainGUIViewModel.PlayerInfoList?.Any(p => p.ClientID == Client._client.Id) == true)
            {
                BasicIoC.MainGUIViewModel.PlayerInfoList.First(p => p.ClientID == Client._client.Id).Level = Enum.GetName(typeof(LevelID), result);
            }
            CurrentLevelId = result;
        }

        public void ObjectiveCountSet() 
        {
            int currentCountMax = 16;
            while(currentCountMax != 8)
            {
                int objectiveCounterAddr = PointerCalculations.GetPointerAddress(0x26A4B0, new int[] { 0x6E });
                ProcessHandler.TryRead(objectiveCounterAddr, out byte result, false);
                currentCountMax = result;
                if (currentCountMax == 16)
                {
                    ProcessHandler.WriteData(objectiveCounterAddr, BitConverter.GetBytes(8), "Setting koala objective koala count");
                    currentCountMax = 8;
                }
            }
        }
    }
}
