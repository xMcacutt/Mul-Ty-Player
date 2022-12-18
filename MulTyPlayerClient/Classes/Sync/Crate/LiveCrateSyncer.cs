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
            ProcessHandler.WriteData(crateAddress + 0x48, new byte[] {0});
            ProcessHandler.WriteData(crateAddress + 0x114, new byte[] {0});
            byte[] buffer = new byte[1];
            int bytesRead = 0;
            ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), crateAddress + 0x178, buffer, 1, ref bytesRead);
            int opalCount = buffer[0];
            int firstCrateOpalAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x28AB7C), new int[] { 0x4AC, 0x0 });
            Console.WriteLine(opalCount);
            for (int i = 0; i < opalCount; i++)
            {
                int opalAddress = PointerCalculations.GetPointerAddress(crateAddress + 0x150 + (4 * i), 0x0);
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), opalAddress + 0x78, buffer, 1, ref bytesRead);
                if (buffer[0] != 5)
                {
                    ProcessHandler.WriteData(opalAddress + 0x78, BitConverter.GetBytes(1));
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
            byte[] currentData = new byte[SyncHandler.HCrate.CratesPerLevel[Program.HLevel.CurrentLevelId]];
            int address = HSyncObject.LiveObjectAddress;
            byte[] buffer = new byte[1];
            int bytesRead = 0;
            for (int i = 0; i < currentData.Length; i++)
            {
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), address + StateOffset + (ObjectLength * i), buffer, 1, ref bytesRead);
                currentData[i] = buffer[0];
            }
            return currentData;
        }
    }
}
