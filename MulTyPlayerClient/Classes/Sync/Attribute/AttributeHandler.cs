using System.Diagnostics;

namespace MulTyPlayerClient
{
    internal class AttributeHandler : SyncObjectHandler
    {
        public new byte[] GlobalObjectData;

        public AttributeHandler()
        {
            Name = "Attribute";
            CheckState = 1;
            SaveState = 1;
            CounterAddress = SyncHandler.SaveDataBaseAddress + 0xAA4;
            CounterAddressStatic = false;
            SaveSync = new SaveAttributeSyncer();
            SetSyncClasses(SaveSync);
            GlobalObjectData = new byte[21];
            GlobalObjectData[(int)Attributes.GotBoom] = 1;
        }

        public override void CheckObserverChanged()
        {
            int attributeAmount = 21;
            for (int i = 0; i < attributeAmount; i++)
            {
                ProcessHandler.TryRead(CounterAddress + i, out byte result, false, "AttributeHandler::CheckObserverChanged()");
                if (result == 1 && GlobalObjectData[i] == 0)
                {
                    GlobalObjectData[i] = 1;
                    //Console.WriteLine("You have now " + Enum.GetValues(typeof(Attributes)).GetValue(i));
                    Client.HSync.SendDataToServer(0, i, 0, Name);
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

            SaveSync.Sync(0, saveData);

            for (int i = 0; i < attributeAmount; i++)
            {
                if (saveData[i] == 1)
                {
                    GlobalObjectData[i] = 1;
                }
            }
            
        }

        public override void HandleClientUpdate(int null1, int iAttribute, int null2)
        {
            GlobalObjectData[iAttribute] = 1;
            SaveSync.Save(iAttribute, null);
        }
    }
}
