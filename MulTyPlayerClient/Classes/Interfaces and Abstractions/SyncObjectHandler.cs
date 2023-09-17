using System;
using System.Collections.Generic;

namespace MulTyPlayerClient
{
    internal abstract class SyncObjectHandler
    {
        public int ObjectAmount { get; set; }
        public int CheckState { get; set; }
        public int SaveState { get; set; }
        public int CounterByteLength { get; set; }
        public bool CounterAddressStatic { get; set; }
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
            if (level != Client.HLevel.CurrentLevelId) return;
            LiveSync.Collect(iLive);
        }

        public virtual int ReadObserver(int address, int size)
        {
            ProcessHandler.TryRead(address, out int result, CounterAddressStatic);
            if (size == 1)
            {
                result = BitConverter.GetBytes(result)[0];
            }
            return result;
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
                        ProcessHandler.TryRead(address, out iSave, false);
                    }
                    if (GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] != CheckState)
                    {
                        //BasicIoC.LoggerInstance.Write(Name + " number " + iLive + " collected.");
                        GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] = (byte)CheckState;
                        Client.HSync.SendDataToServer(iLive, iSave, Client.HLevel.CurrentLevelId, Name);
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
                if (liveData[i] == CheckState && GlobalObjectData[level][i] != CheckState) GlobalObjectData[level][i] = WriteState;
            }
            if(Client.HLevel.CurrentLevelId == level)
            {
                LiveSync.Sync(liveData, ObjectAmount, CheckState);
                PreviousObjectData = liveData;
                CurrentObjectData = liveData;
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

        public virtual void SetCurrentData()
        {
            CurrentObjectData = GlobalObjectData[Client.HLevel.CurrentLevelId];
            PreviousObjectData = GlobalObjectData[Client.HLevel.CurrentLevelId];
        }
    }
}
