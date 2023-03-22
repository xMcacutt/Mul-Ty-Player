using System;
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
            SetMemAddrs();
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

        public async override Task HandleClientUpdate(int iLive, int iSave, int level)
        {
            if (GlobalObjectData[level][iLive] == 0) return;
            GlobalObjectData[level][iLive] = (byte)CheckState;
            if (level != Client.HLevel.CurrentLevelId) return;
            //Console.WriteLine("Collecting Crate: " + iLive);
            await LiveSync.Collect(iLive);
        }

        public async override Task Sync(int level, byte[] liveData, byte[] saveData)
        {
            for (int i = 0; i < CratesPerLevel[level]; i++)
            {
                if (liveData[i] == 0 && GlobalObjectData[level][i] == 1) GlobalObjectData[level][i] = 0;
            }
            if (Client.HLevel.CurrentLevelId == level)
            {
                PreviousObjectData = liveData;
                CurrentObjectData = liveData;
                await LiveSync.Sync(liveData, CratesPerLevel[level], CheckState);
            }
        }

        public override bool CheckObserverCondition(byte previousState, byte currentState)
        {
            return previousState == 1 && currentState == 0;
        }

        public async override void SetMemAddrs()
        {
            CounterAddress = await PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x0028A8E8), new int[] { 0x390 });
            await ProcessHandler.WriteDataAsync(CounterAddress, new byte[] { 0, 0, 0, 0 });
            LiveObjectAddress = 
                Client.HLevel.CurrentLevelId == 10 ?
                await PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x255190), new int[] { 0x0 }) :
                await PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x254CB8), new int[] { 0x0 });
        }

        public async override Task CheckObserverChanged()
        {
            if (!Client.HLevel.MainStages.Contains(Client.HLevel.CurrentLevelId)) return;
            ObserverState = await ReadObserver(CounterAddress, CounterByteLength);
            if (PreviousObserverState == ObserverState || ObserverState == 0) return;
            PreviousObserverState = ObserverState;
            CurrentObjectData = await LiveSync.ReadData();
            for (int iLive = 0; iLive < CratesPerLevel[Client.HLevel.CurrentLevelId]; iLive++)
            {
                if (CheckObserverCondition(PreviousObjectData[iLive], CurrentObjectData[iLive]))
                {
                    PreviousObjectData[iLive] = CurrentObjectData[iLive] = WriteState;
                    if (GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] != CheckState)
                    {
                        //Console.WriteLine(Name + " number " + iLive + " collected.");
                        GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] = (byte)CheckState;
                        Client.HSync.SendDataToServer(iLive, iLive, Client.HLevel.CurrentLevelId, Name);
                    }
                }
            }
        }
    }
}
