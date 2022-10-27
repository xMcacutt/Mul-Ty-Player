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

        public void CheckAttributeChange()
        {
            //Console.WriteLine($"Current TE Count = {CollectibleCounts[0]}\nPrevious TE Count = {PreviousCollectibleCounts[0]}");
            AttributeData = ReadAttributeData();
            if (!Enumerable.SequenceEqual(PreviousAttributeData, AttributeData))
            {
                HSync.UpdateServerData(0, AttributeData, "Attribute");
                PreviousAttributeData = AttributeData;
            }
        }

        [MessageHandler((ushort)MessageID.ClientAttributeDataUpdate)]
        public static void UpdateClientWithAttr(Message message)
        {
            byte[] bytes = message.GetBytes();
            Program.HAttribute.AttributeData = bytes;
            SyncHandler._lastReceivedServerData = bytes;
            ProcessHandler.WriteData(Program.HAttribute.AttributeDataBaseAddress, Program.HAttribute.AttributeData);
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
