﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class CrateHandler : SyncObjectHandler
    {
        public int[] CratesPerLevel = { 0, 0, 0, 0, 31, 16, 24, 0, 24, 12, 300, 0, 6, 34, 43 };
        public CrateHandler()
        {
            Name = "Crate";
            WriteState = 0;
            CheckState = 0;
            CounterAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x0028A8E8), 0x390);
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
            GlobalObjectData[level][iLive] = (byte)CheckState;
            if (level != Program.HLevel.CurrentLevelId) return;
            Console.WriteLine("Collecting Crate: " + iLive);
            LiveSync.Collect(iLive);
        }

        public override void Sync(int level, byte[] liveData, byte[] saveData)
        {
            for (int i = 0; i < CratesPerLevel[level]; i++)
            {
                if (liveData[i] == CheckState && GlobalObjectData[level][i] != CheckState) GlobalObjectData[level][i] = WriteState;
            }
            if (Program.HLevel.CurrentLevelId == level)
            {
                LiveSync.Sync(liveData, CratesPerLevel[level], CheckState);
                PreviousObjectData = liveData;
                CurrentObjectData = liveData;
            }
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return previousState == 1 && currentState == 0;
        }

        public override void SetMemAddrs()
        {
            LiveObjectAddress = 
                Program.HLevel.CurrentLevelId == 10 ?
                PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x255190), 0x0):
                PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x254CB8), 0x0);
        }

        public override void CheckObserverChanged()
        {
            ObserverState = ReadObserver(CounterAddress, CounterByteLength);
            if (PreviousObserverState == ObserverState || ObserverState == 0) return;

            PreviousObserverState = ObserverState;
            CurrentObjectData = LiveSync.ReadData();
            for (int iLive = 0; iLive < CratesPerLevel[Program.HLevel.CurrentLevelId]; iLive++)
            {
                if (CheckObserverCondition(PreviousObjectData[iLive], CurrentObjectData[iLive]))
                {
                    PreviousObjectData[iLive] = CurrentObjectData[iLive] = WriteState;
                    if (GlobalObjectData[Program.HLevel.CurrentLevelId][iLive] != CheckState)
                    {
                        Console.WriteLine(Name + " number " + iLive + " collected.");
                        GlobalObjectData[Program.HLevel.CurrentLevelId][iLive] = (byte)CheckState;
                        Program.HSync.SendDataToServer(iLive, iLive, Program.HLevel.CurrentLevelId, Name);
                    }
                }
            }
        }
    }
}
