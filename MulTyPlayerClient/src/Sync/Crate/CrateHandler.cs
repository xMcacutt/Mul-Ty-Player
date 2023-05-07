using System.Linq;
using MulTyPlayerClient.Memory;
using MulTyPlayerClient.Networking;

namespace MulTyPlayerClient.Sync
{
    internal class CrateHandler : SyncObjectHandler
    {
        public CrateHandler()
        {
            Name = "Crate";
            WriteState = 0;
            CheckState = 0;
            SetMemAddrs();
            CounterAddress = Addresses.GetPointerAddress(0x0028A8E8, new int[] { 0x390 });
            CounterAddressStatic = false;
            CounterByteLength = 4;
            CurrentObjectData = Enumerable.Repeat((byte)1, 300).ToArray();
            PreviousObjectData = Enumerable.Repeat((byte)1, 300).ToArray();
            GlobalObjectData = new()
            {
                {4, Enumerable.Repeat((byte)1, 31).ToArray()},
                {5, Enumerable.Repeat((byte)1, 16).ToArray()},
                {6, Enumerable.Repeat((byte)1, 24).ToArray()},
                {8, Enumerable.Repeat((byte)1, 24).ToArray()},
                {9, Enumerable.Repeat((byte)1, 12).ToArray()},
                {10, Enumerable.Repeat((byte)1, 300).ToArray()},
                {12, Enumerable.Repeat((byte)1, 6).ToArray()},
                {13, Enumerable.Repeat((byte)1, 34).ToArray()},
                {14, Enumerable.Repeat((byte)1, 43).ToArray()},
            };
            LiveSync = new LiveCrateSyncer(this);
            SetSyncClasses(LiveSync);
        }

        public override void HandleClientUpdate(int iLive, int iSave, int level)
        {
            if (GlobalObjectData[level][iLive] == 0) return;
            GlobalObjectData[level][iLive] = (byte)CheckState;
            if (level != LevelHandler.CurrentLevelId) return;
            LiveSync.Collect(iLive);
        }

        public override void Sync(int level, byte[] liveData, byte[] saveData)
        {
            int crateCount = Levels.GetLevelData(level).CrateCount;
            for (int i = 0; i < crateCount; i++)
            {
                if (liveData[i] == 0 && GlobalObjectData[level][i] == 1) GlobalObjectData[level][i] = 0;
            }
            if (LevelHandler.CurrentLevelId == level)
            {
                PreviousObjectData = liveData;
                CurrentObjectData = liveData;
                LiveSync.Sync(liveData, crateCount, CheckState);
            }
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return previousState == 1 && currentState == 0;
        }

        public override void SetMemAddrs()
        {
            CounterAddress = Addresses.GetPointerAddress(0x0028A8E8, new int[] { 0x390 });
            ProcessHandler.WriteData(CounterAddress, new byte[] { 0, 0, 0, 0 }, "Setting crate anim counter to 0");
            LiveObjectAddress =
                LevelHandler.CurrentLevelId == 10 ?
                Addresses.GetPointerAddress(0x255190, new int[] { 0x0 }) :
                Addresses.GetPointerAddress(0x254CB8, new int[] { 0x0 });
        }

        public override void CheckObserverChanged()
        {
            if (Levels.GetLevelData(LevelHandler.CurrentLevelId).IsMainStage)
                return;

            ObserverState = ReadObserver(CounterAddress, CounterByteLength);

            if (PreviousObserverState == ObserverState || ObserverState == 0)
                return;

            PreviousObserverState = ObserverState;
            CurrentObjectData = LiveSync.ReadData();
            int crateCount = Levels.GetLevelData(LevelHandler.CurrentLevelId).CrateCount;
            for (int iLive = 0; iLive < crateCount; iLive++)
            {
                if (CheckObserverCondition(PreviousObjectData[iLive], CurrentObjectData[iLive]))
                {
                    PreviousObjectData[iLive] = CurrentObjectData[iLive] = WriteState;
                    if (GlobalObjectData[LevelHandler.CurrentLevelId][iLive] != CheckState)
                    {
                        GlobalObjectData[LevelHandler.CurrentLevelId][iLive] = (byte)CheckState;
                        Replication.HSync.SendDataToServer(iLive, iLive, LevelHandler.CurrentLevelId, Name);
                    }
                }
            }
        }
    }
}
