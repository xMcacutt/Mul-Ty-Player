using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;

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
        public int CounterAddress { get; set; }
        public int LiveObjectAddress { get; set; }
        public string Name { get; set; }

        public Dictionary<int, byte[]> GlobalObjectData;

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
            GlobalObjectData[level][index] = BitConverter.GetBytes(CheckState)[0];
            SaveSync.Save(index, level);
            if (level != Program.HLevel.CurrentLevelId) return;
            LiveSync.Collect(index);
        }

        public virtual int ReadObserver(int address)
        {
            return BitConverter.ToInt32(ProcessHandler.ReadData("observer read", PointerCalculations.AddOffset(address), 4), 0);
        }

        public virtual void CheckObserverChanged()
        {
            ObserverState = ReadObserver(CounterAddress);
            if (PreviousObserverState == ObserverState || ObserverState == 0) return;
            PreviousObserverState = ObserverState;
            CurrentObjectData = LiveSync.ReadData();
            for (int i = 0; i < CurrentObjectData.Length; i++)
            {
                if (CheckObserverCondition(PreviousObjectData[i], CurrentObjectData[i]))
                {
                    PreviousObjectData[i] = CurrentObjectData[i] = WriteState;
                    if (GlobalObjectData[Program.HLevel.CurrentLevelId][i] == CheckState) return;
                    GlobalObjectData[Program.HLevel.CurrentLevelId][i] = BitConverter.GetBytes(CheckState)[0];
                    Program.HSync.SendDataToServer(i, Program.HLevel.CurrentLevelId, Name);
                }
            }
        }

        public abstract bool CheckObserverCondition(byte previousState, byte currentState);

        public virtual void Sync(int level, byte[] data)
        {
            if(Program.HLevel.CurrentLevelId == level)
            {
                CurrentObjectData = data;
                PreviousObjectData = data;
                LiveSync.Sync(data, ObjectAmount, CheckState);
            }
            SaveSync.Sync(level, data);
        }
    }
}
