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
            int crateAddress = HSyncObject.LiveObjectAddress + (index * ObjectLength);
            ProcessHandler.WriteData(crateAddress + 0x48, new byte[] {0}, "Setting crate collision to false");
            ProcessHandler.WriteData(crateAddress + 0x114, new byte[] {0}, "Setting crate visibility to false");
            int opalCount = ProcessHandler.ReadData(crateAddress + 0x178, 1, "Getting crate opal count")[0];
            for (int i = 0; i < opalCount; i++)
            {
                int opalAddress = PointerCalculations.GetPointerAddress(crateAddress + 0x150 + (4 * i), new int[] { 0x0 });
                if (ProcessHandler.ReadData(opalAddress + 0x78, 1, $"Getting opal address from crate data {i} / {opalCount}")[0] != 5)
                {
                    ProcessHandler.WriteData(opalAddress + 0x78, BitConverter.GetBytes(1), $"Spawning opal from crate {i} / {opalCount}");
                }
            }
        }

        public override void Sync(byte[] bytes, int amount, int checkState)
        {
            for (int i = 0; i < amount; i++)
            {
                if (bytes[i] == checkState) Collect(i);
            }
        }

        public override byte[] ReadData()
        {
            byte[] currentData = new byte[SyncHandler.HCrate.CratesPerLevel[Client.HLevel.CurrentLevelId]];
            int address = HSyncObject.LiveObjectAddress;
            for (int i = 0; i < currentData.Length; i++)
            {
                currentData[i] = ProcessHandler.ReadData(address + StateOffset + (ObjectLength * i), 1, $"Checking state of all crates {i} / {currentData.Length}")[0];
            }
            return currentData;
        }
    }
}
