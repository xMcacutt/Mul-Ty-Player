using System;
using System.Collections.Generic;
using MulTyPlayerClient.Networking;

namespace MulTyPlayerClient.Sync
{
    internal class RCHandler : SyncObjectHandler
    {
        public new Dictionary<int, byte> GlobalObjectData;

        public RCHandler()
        {
            Name = "RC";
            CheckState = 1;
            SaveState = 1;
            CounterAddress = SyncHandler.SaveDataBaseAddress + 0xA84;
            CounterAddressStatic = false;
            SaveSync = new SaveRCSyncer();
            SetSyncClasses(SaveSync);
            GlobalObjectData = new Dictionary<int, byte>();
            foreach (int i in Enum.GetValues(typeof(RCData))) GlobalObjectData.Add(i, 0);
        }

        public override void CheckObserverChanged()
        {
            int dataAmount = 12;
            for (int i = 0; i < dataAmount; i++)
            {
                Memory.ProcessHandler.TryRead(CounterAddress + i, out byte dataState, false);
                if (dataState == 1 && GlobalObjectData[i] == 0)
                {
                    GlobalObjectData[i] = 1;
                    Replication.HSync.SendDataToServer(0, i, 0, Name);
                }
            }
        }

        public override void SetMemAddrs()
        {
            CounterAddress = SyncHandler.SaveDataBaseAddress + 0xA84;
        }

        public override void Sync(int null1, byte[] null2, byte[] saveData)
        {
            int dataAmount = 12;
            for (int i = 0; i < dataAmount; i++)
            {
                if (saveData[i] == 1)
                {
                    GlobalObjectData[i] = 1;
                }
            }
            SaveSync.Sync(0, saveData);
        }

        public override void HandleClientUpdate(int null1, int iAttribute, int null2)
        {
            GlobalObjectData[iAttribute] = 1;
            SaveSync.Save(iAttribute, null);
        }
    }
}
