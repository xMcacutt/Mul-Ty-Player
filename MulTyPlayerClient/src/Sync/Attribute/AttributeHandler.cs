using System;
using System.Collections.Generic;
using MulTyPlayerClient.Networking;

namespace MulTyPlayerClient.Sync
{
    internal class AttributeHandler : SyncObjectHandler
    {
        public new Dictionary<int, byte> GlobalObjectData;

        public AttributeHandler()
        {
            Name = "Attribute";
            CheckState = 1;
            SaveState = 1;
            CounterAddress = SyncHandler.SaveDataBaseAddress + 0xAA4;
            CounterAddressStatic = false;
            SaveSync = new SaveAttributeSyncer();
            SetSyncClasses(SaveSync);
            GlobalObjectData = new Dictionary<int, byte>();
            foreach (int i in Enum.GetValues(typeof(Attributes))) GlobalObjectData.Add(i, 0);
            GlobalObjectData[(int)Attributes.GotBoom] = 1;
        }

        public override void CheckObserverChanged()
        {
            int attributeAmount = 21;
            for (int i = 0; i < attributeAmount; i++)
            {
                Memory.ProcessHandler.TryRead(CounterAddress + i, out byte result, false);
                if (result == 1 && GlobalObjectData[i] == 0)
                {
                    GlobalObjectData[i] = 1;
                    //Console.WriteLine("You have now " + Enum.GetValues(typeof(Attributes)).GetValue(i));
                    Replication.HSync.SendDataToServer(0, i, 0, Name);
                }
            }
        }

        public override void SetMemAddrs()
        {
            CounterAddress = SyncHandler.SaveDataBaseAddress + 0xAA4;
        }

        public override void Sync(int null1, byte[] null2, byte[] saveData)
        {
            int attributeAmount = 21;
            for (int i = 0; i < attributeAmount; i++)
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
