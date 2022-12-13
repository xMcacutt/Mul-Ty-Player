using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class LiveCrateSyncer : LiveDataSyncer
    {

        public LiveCrateSyncer(SyncObjectHandler hCrate)
        {
            HSyncObject = hCrate;
            StateOffset = 0x114;
            ObjectLength = 0x1C0;
        }

        public override void Collect(int index)
        {
            if (HSyncObject.GlobalObjectData[Program.HLevel.CurrentLevelId][index] == 0) return;
            int crateAddress = HSyncObject.LiveObjectAddress + (index * ObjectLength);
            ProcessHandler.WriteData(crateAddress + 0x48, new byte[] {0});
            ProcessHandler.WriteData(crateAddress + 0x114, new byte[] {0});
            int opalCount = ProcessHandler.ReadData("crate opal count", crateAddress + 0x178, 1)[0];
            int firstCrateOpalAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x28AB7C), new int[] { 0x4AC, 0x0 });
            for (int i = 0; i < opalCount; i++)
            {
                int opalAddress = PointerCalculations.GetPointerAddress(crateAddress + 0x150 + (4 * i), 0x0);
                int opalIndex = ((opalAddress - firstCrateOpalAddress) / ObjectLength) + (300 - SyncHandler.HOpal.CrateOpalsPerLevel[Program.HLevel.CurrentLevelId]);
                if (Program.HSync.SyncObjects["Opal"].GlobalObjectData[Program.HLevel.CurrentLevelId][opalIndex] != 5)
                {
                    ProcessHandler.WriteData(opalAddress + 0x78, BitConverter.GetBytes(1));
                }
            }
        }

        public override byte[] ReadData()
        {
            byte[] currentData = new byte[SyncHandler.HCrate.CratesPerLevel[Program.HLevel.CurrentLevelId]];
            int address = HSyncObject.LiveObjectAddress;
            Console.WriteLine("Crates");
            for (int i = 0; i < currentData.Length; i++)
            {
                currentData[i] = ProcessHandler.ReadData("current object read", address + StateOffset + (ObjectLength * i), 1)[0];
                Console.Write(currentData[i]);
            }
            Console.WriteLine("");
            return currentData;
        }
    }
}
