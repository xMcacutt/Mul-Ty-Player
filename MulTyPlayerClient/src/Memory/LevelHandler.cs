using MulTyPlayerClient.GUI;
using MulTyPlayerClient.Logging;
using MulTyPlayerClient.Networking;
using MulTyPlayerClient.Sync;
using System;

namespace MulTyPlayerClient.Memory
{
    internal class LevelHandler
    {
        static KoalaReplication HKoala => Replication.KoalaHandler;
        static SyncHandler HSync => Replication.HSync;

        public static int CurrentLevelId
        {
            get
            {
                return currentLevelId;
            }

            set
            {
                if (currentLevelId != value)
                {
                    currentLevelId = value;
                    OnLevelChange?.Invoke(value);
                    DoLevelSetup();
                }
            }
        }
        private static int currentLevelId;

        public static Action<int> OnLevelChange = delegate { };

        public static void DoLevelSetup()
        {
            Log.WriteDebug("Doing level setup");
            LevelData lData = Levels.GetLevelData(currentLevelId);
            HSync.SetCurrentData(lData.IsMainStage);
            HSync.SetMemAddrs();
            HSync.RequestSync();
            HSync.ProtectLeaderboard();
            HKoala.SetBaseAddress();
            HKoala.SetCoordAddrs();
            if (lData.HasKoalas)
                ObjectiveCountSet();
            OnLevelChange?.Invoke(currentLevelId);
        }

        public void GetCurrentLevel()
        {
            ProcessHandler.TryRead(0x280594, out int levelId, true);
            CurrentLevelId = levelId;
        }

        public static void ObjectiveCountSet()
        {
            int currentCountMax = 16;
            while (currentCountMax != 8)
            {
                int objectiveCounterAddr = Addresses.GetPointerAddress(0x26A4B0, new int[] { 0x6E });
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
