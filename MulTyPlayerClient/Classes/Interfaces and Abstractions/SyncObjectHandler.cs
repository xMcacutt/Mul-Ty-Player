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
        public int SaveState { get; set; }
        public int CounterByteLength { get; set; }
        public bool SeparateID { get; set; }
        public int IDOffset { get; set; }
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

        public virtual void SetSyncClasses(LiveDataSyncer LiveSync)
        {
            this.LiveSync = LiveSync;
        }

        public virtual void SetSyncClasses(SaveDataSyncer SaveSync)
        {
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

        public virtual int ReadObserver(int address, int size)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            ProcessHandler.ReadProcessMemory((int)ProcessHandler.HProcess, address, buffer, size, ref bytesRead);
            return size == 4 ? BitConverter.ToInt32(buffer, 0) : buffer[0];
        }

        public virtual void CheckObserverChanged()
        {
            ObserverState = ReadObserver(CounterAddress, CounterByteLength);
            if (PreviousObserverState == ObserverState || ObserverState == 0) return;
         
            PreviousObserverState = ObserverState;
            CurrentObjectData = LiveSync.ReadData();
            for (int i = 0; i < CurrentObjectData.Length; i++)
            {
                if (CheckObserverCondition(PreviousObjectData[i], CurrentObjectData[i]))
                {
                    PreviousObjectData[i] = CurrentObjectData[i] = WriteState;
                    if (SeparateID) 
                    {
                        int address = LiveObjectAddress + (i * LiveSync.ObjectLength) + IDOffset;
                        int bytesRead = 0;
                        byte[] buffer = new byte[4];
                        ProcessHandler.ReadProcessMemory((int)ProcessHandler.HProcess, address, buffer, 4, ref bytesRead);
                        i = BitConverter.ToInt32(buffer, 0);
                    }
                    if (GlobalObjectData[Program.HLevel.CurrentLevelId][i] == CheckState) return;
                    Console.WriteLine(Name + " number " + i + " collected.");
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
            SaveSync.Sync(level, ConvertLiveToSave(level, data));
        }

        public virtual byte[] ConvertLiveToSave(int level, byte[] liveData)
        {
            byte[] saveData = new byte[liveData.Length];
            for (int i = 0; i < liveData.Length; i++)
            {
                if (liveData[i] == CheckState)
                {
                    saveData[i] = (byte)SaveState;
                }
            }
            return saveData;
        }
    }
}
