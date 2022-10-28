using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class AttributeHandler
    {
        IntPtr HProcess => ProcessHandler.HProcess;
        SyncHandler HSync => Program.HSync;

        public byte[] AttributeData;
        public byte[] PreviousAttributeData;
        public int AttributeDataBaseAddress;

        public AttributeHandler()
        {
            AttributeData = new byte[26];
            PreviousAttributeData = new byte[26];
            AttributeDataBaseAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0xAB4);
            AttributeData = ReadAttributeData();
        }
        public void HandleClientUpdate(byte[] data)
        {
            AttributeData = data;
            ProcessHandler.WriteData(AttributeDataBaseAddress, AttributeData);
        }

        public void CheckAttributeChange()
        {
            AttributeData = ReadAttributeData();
            if (!Enumerable.SequenceEqual(PreviousAttributeData, AttributeData))
            {
                HSync.UpdateServerData(0, AttributeData, "Attribute");
                PreviousAttributeData = AttributeData;
            }
        }

        public byte[] ReadAttributeData()
        {
            int bytesRead = 0;
            byte[] buffer = new byte[23];
            ProcessHandler.ReadProcessMemory((int)HProcess, AttributeDataBaseAddress, buffer, 26, ref bytesRead);
            return buffer;
        }
    }
}
