using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using System;

namespace MulTyPlayerClient
{
    internal class LevelHandler
    {
        static KoalaHandler HKoala => Client.HKoala;
        static SyncHandler HSync => Client.HSync;

        public int CurrentLevelId
        {
            get => currentLevelId;
            set
            {
                if (currentLevelId != value)
                {
                    currentLevelId = value;
                    CurrentLevelData = Levels.GetLevelData(value);
                }
            }
        }
        private int currentLevelId;

        public Action<int> OnLevelChange = delegate { };

        public LevelData CurrentLevelData;
        
        public void DoLevelSetup()
        {
            GetCurrentLevel();
            HSync.SetCurrentData(CurrentLevelData.IsMainStage, CurrentLevelData.FrameCount != 0);
            HSync.SetMemAddrs();
            HSync.RequestSync();
            HSync.ProtectLeaderboard();
            HKoala.SetBaseAddress();
            HKoala.SetCoordinateAddresses();
            if (CurrentLevelData.HasKoalas)
                ObjectiveCountSet();
            OnLevelChange?.Invoke(currentLevelId);
        }

        public void GetCurrentLevel()
        {
            ProcessHandler.TryRead(0x280594, out int levelId, true, "LevelHandler::GetCurrentLevel()");

            if (ModelController.Lobby.TryGetPlayerInfo(Client._client.Id, out PlayerInfo playerInfo))
            {
                playerInfo.Level = Levels.GetLevelData(levelId).Code;
            }
            CurrentLevelId = levelId;
        }

        public static void ObjectiveCountSet() 
        {
            int currentCountMax = 16;
            while(currentCountMax != 8)
            {
                int objectiveCounterAddr = PointerCalculations.GetPointerAddress(0x26A4B0, new int[] { 0x6E });
                ProcessHandler.TryRead(objectiveCounterAddr, out byte result, false, "LevelHandler::ObjectiveCountSet()");
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
