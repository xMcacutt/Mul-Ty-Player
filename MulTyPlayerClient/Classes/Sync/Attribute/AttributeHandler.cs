using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MulTyPlayerClient
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
            SaveSync = new SaveAttributeSyncer();
            SetSyncClasses(SaveSync);
            GlobalObjectData = new Dictionary<int, byte>();
            foreach (int i in Enum.GetValues(typeof(Attributes))) GlobalObjectData.Add(i, 0);
            GlobalObjectData[(int)Attributes.GotBoom] = 1;
        }

        public async override Task CheckObserverChanged()
        {
            int attributeAmount = 21;
            for (int i = 0; i < attributeAmount; i++)
            {
                if ((await ProcessHandler.ReadDataAsync(SyncHandler.SaveDataBaseAddress + 0xAA4 + i, 1))[0] == 1 && GlobalObjectData[i] == 0)
                {
                    GlobalObjectData[i] = 1;
                    //Console.WriteLine("You have now " + Enum.GetValues(typeof(Attributes)).GetValue(i));
                    Client.HSync.SendDataToServer(0, i, 0, Name);
                }
            }
        }

        public async override Task Sync(int null1, byte[] null2, byte[] saveData)
        {
            int attributeAmount = 21;
            for (int i = 0; i < attributeAmount; i++)
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
