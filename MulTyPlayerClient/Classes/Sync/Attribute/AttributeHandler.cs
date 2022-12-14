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

        public override void CheckObserverChanged()
        {
            int attributeAmount = 21;
            byte[] observerData = new byte[attributeAmount];
            for (int i = 0; i < attributeAmount; i++)
            {
                byte[] buffer = new byte[1];
                int bytesRead = 0;
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), SyncHandler.SaveDataBaseAddress + 0xAA4 + i, buffer, 1, ref bytesRead);
                if (buffer[0] == 1 && GlobalObjectData[i] == 0)
                {
                    GlobalObjectData[i] = 1;
                   //Console.WriteLine("You have now " + Enum.GetValues(typeof(Attributes)).GetValue(i));
                    Program.HSync.SendDataToServer(0, i, 0, Name);
                }
            }
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
