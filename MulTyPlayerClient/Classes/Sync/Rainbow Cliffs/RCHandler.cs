using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MulTyPlayerClient
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
            SaveSync = new SaveRCSyncer();
            SetSyncClasses(SaveSync);
            GlobalObjectData = new Dictionary<int, byte>();
            foreach (int i in Enum.GetValues(typeof(RCData))) GlobalObjectData.Add(i, 0);
        }

        public async override Task CheckObserverChanged()
        {
            int dataAmount = 12;
            for (int i = 0; i < dataAmount; i++)
            {
                if ((await ProcessHandler.ReadDataAsync(CounterAddress + i, 1))[0] == 1 && GlobalObjectData[i] == 0)
                {
                    GlobalObjectData[i] = 1;
                    Client.HSync.SendDataToServer(0, i, 0, Name);
                }
            }
        }

        public async override Task Sync(int null1, byte[] null2, byte[] saveData)
        {
            int dataAmount = 12;
            for (int i = 0; i < dataAmount; i++)
            {
                if (saveData[i] == 1)
                {
                    GlobalObjectData[i] = 1;
                }
            }
            await SaveSync.Sync(0, saveData);
        }

        public async override Task HandleClientUpdate(int null1, int iAttribute, int null2)
        {
            GlobalObjectData[iAttribute] = 1;
            await SaveSync.Save(iAttribute, null);
        }
    }
}
