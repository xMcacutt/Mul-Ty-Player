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

        public virtual void SetMemAddrs() { }

        public virtual void HandleClientUpdate(int iLive, int iSave, int level)
        {
            GlobalObjectData[level][iLive] = (byte)CheckState;
            SaveSync.Save(iSave, level);
            if (level != Program.HLevel.CurrentLevelId) return;
            LiveSync.Collect(iLive);
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
            int iSave;
            for (int iLive = 0; iLive < CurrentObjectData.Length; iLive++)
            {
                if (CheckObserverCondition(PreviousObjectData[iLive], CurrentObjectData[iLive]))
                {
                    iSave = iLive;
                    PreviousObjectData[iLive] = CurrentObjectData[iLive] = WriteState;
                    if (SeparateID) 
                    {
                        int address = LiveObjectAddress + (iLive * LiveSync.ObjectLength) + IDOffset;
                        int bytesRead = 0;
                        byte[] buffer = new byte[4];
                        ProcessHandler.ReadProcessMemory((int)ProcessHandler.HProcess, address, buffer, 4, ref bytesRead);
                        iSave = BitConverter.ToInt32(buffer, 0);
                    }
                    if (GlobalObjectData[Program.HLevel.CurrentLevelId][iLive] != CheckState)
                    {
                        Console.WriteLine(Name + " number " + iLive + " collected.");
                        GlobalObjectData[Program.HLevel.CurrentLevelId][iLive] = (byte)CheckState;
                        Program.HSync.SendDataToServer(iLive, iSave, Program.HLevel.CurrentLevelId, Name);
                    }
                }
            }
        }

        public virtual bool CheckObserverCondition(byte previousState, byte currentState) { return false; }

        public virtual void Sync(int level, byte[] liveData, byte[] saveData)
        {
            SaveSync.Sync(level, ConvertSave(level, saveData));
            for(int i = 0; i < ObjectAmount; i++)
            {
                if (liveData[i] == CheckState) GlobalObjectData[level][i] = WriteState;
            }
            if(Program.HLevel.CurrentLevelId == level)
            {
                CurrentObjectData = liveData;
                PreviousObjectData = liveData;
                LiveSync.Sync(liveData, ObjectAmount, CheckState);
            }
        }

        public virtual byte[] ConvertSave(int level, byte[] data)
        {
            byte[] output = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == CheckState)
                {
                    output[i] = (byte)SaveState;
                }
            }
            return output;
        }
    }
}
