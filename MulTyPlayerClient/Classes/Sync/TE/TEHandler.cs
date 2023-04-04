using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class TEHandler : SyncObjectHandler
    {
        public TEHandler()
        {
            Name = "TE";
            WriteState = 5;
            CheckState = 5;
            SaveState = 1;
            ObjectAmount = 8;
            SeparateID = true;
            IDOffset = 0x6C;
            CounterAddress = PointerCalculations.GetPointerAddress(0x00288730, new int[] { 0xD });
            CounterAddressStatic = false;
            CounterByteLength = 1;
            PreviousObjectData = new byte[ObjectAmount];
            CurrentObjectData = new byte[ObjectAmount];
            LiveSync = new LiveTESyncer(this);
            SaveSync = new SaveTESyncer();
            SetSyncClasses(LiveSync, SaveSync);
            GlobalObjectData = new Dictionary<int, byte[]>();
            foreach (int i in Client.HLevel.MainStages) GlobalObjectData.Add(i, new byte[ObjectAmount]);
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return (previousState < 3 && currentState > 3);
        }

        public override void HandleClientUpdate(int iLive, int iSave, int level)
        {
            GlobalObjectData[level][iLive] = (byte)CheckState;
            SaveSync.Save(iSave, level);
            if (level != Client.HLevel.CurrentLevelId) return;
            if (iSave == 0) { Client.HSync.SyncObjects["Crate"].GlobalObjectData[level] = new byte[CrateHandler.CratesPerLevel[level]]; }
            LiveSync.Collect(iLive);
        }

        public override void CheckObserverChanged()
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
                        if (iSave == 0) { Client.HSync.SyncObjects["Crate"].GlobalObjectData[Client.HLevel.CurrentLevelId] = new byte[CrateHandler.CratesPerLevel[Client.HLevel.CurrentLevelId]]; }
                        Client.HSync.SendDataToServer(iLive, iSave, Client.HLevel.CurrentLevelId, Name);
                    }
                }
            }
        }

        public  override void SetMemAddrs()
        {
            CounterAddress = PointerCalculations.GetPointerAddress(0x00288730, new int[] { 0xD });
            LiveObjectAddress = PointerCalculations.GetPointerAddress(0x270280, new int[] { 0x0 });
        }
    }
}
