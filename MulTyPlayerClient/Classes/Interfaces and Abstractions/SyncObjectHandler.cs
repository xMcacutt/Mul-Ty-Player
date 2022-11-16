using System;
using System.ComponentModel.Design;
using System.Linq;

namespace MulTyPlayerClient
{
    internal abstract class SyncObjectHandler
    {
        public int ObjectAmount { get; set; }
        public int CheckState { get; set; }
        public byte[] CurrentObjectData { get; set; }
        public byte[] PreviousObjectData { get; set; }
        public int ObserverState { get; set; }
        public int PreviousObserverState { get; set; }
        public byte WriteState { get; set; }
        public string Name { get; set; }

        public LiveDataSyncer LiveSync;
        public SaveDataSyncer SaveSync;

        public virtual void SetSyncClasses(LiveDataSyncer LiveSync, SaveDataSyncer SaveSync)
        {
            this.LiveSync = LiveSync;
            this.SaveSync = SaveSync;
        }

        public abstract void SetMemAddrs();

        public virtual void HandleClientUpdate(int index, int level)
        {
            SaveSync.Save(index);
            if (level != Program.HLevel.CurrentLevelId) return;
            LiveSync.Collect(index);
        }

        public virtual int ReadObserver(int address)
        {
            return BitConverter.ToInt32(ProcessHandler.ReadData("collectible count", PointerCalculations.AddOffset(address), 4), 0);
        }

        public virtual void CheckObserverChanged(int address)
        {
            if (!Program.HLevel.MainStages.Contains(Program.HLevel.CurrentLevelId)) return;
            ObserverState = ReadObserver(address);
            if (PreviousObserverState == ObserverState || ObserverState == 0) return;
            PreviousObserverState = ObserverState;
            CurrentObjectData = LiveSync.ReadData();
            for (int i = 0; i < CurrentObjectData.Length; i++)
            {
                if (CheckObserverCondition(PreviousObjectData[i], CurrentObjectData[i]))
                {
                    PreviousObjectData[i] = CurrentObjectData[i] = WriteState;
                    Program.HSync.SendDataToServer(i, Program.HLevel.CurrentLevelId, Name);
                }
            }
        }

        public abstract bool CheckObserverCondition(byte previousState, byte currentState);

        public virtual void SyncAll(byte[] data)
        {
            CurrentObjectData = data;
            PreviousObjectData = data;
            LiveSync.SyncAll(data, ObjectAmount, CheckState);
            SaveSync.SyncAll(data);
        }

        public virtual void ResetSync() 
        {
            CurrentObjectData = new byte[ObjectAmount];
            PreviousObjectData = new byte[ObjectAmount];
        }
    }
}
