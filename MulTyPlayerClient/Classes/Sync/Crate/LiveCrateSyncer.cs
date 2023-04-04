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
            ProcessHandler.TryRead(crateAddress + 0x178, out byte opalCount, false);
            for (int i = 0; i < opalCount; i++)
            {
                ProcessHandler.TryRead(crateAddress + 0x150 + (4 * i), out IntPtr opalAddress, false);
                ProcessHandler.TryRead(opalAddress + 0x78, out byte opalState, false);
                if (opalState == 0)
                {
                    ProcessHandler.WriteData((int)(opalAddress + 0x78), BitConverter.GetBytes(1), $"Spawning opal from crate {i} / {opalCount}");
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
                ProcessHandler.TryRead(address + StateOffset + (ObjectLength * i), out currentData[i], false);
            }
            return currentData;
        }
    }
}
