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
        int CrateOpalsAddress;
        public LiveCrateSyncer(SyncObjectHandler hCrate)
        {
            HSyncObject = hCrate;
        }

        public override void Collect(int index)
        {
            int crateAddress = HSyncObject.LiveObjectAddress + (index * 0x1C0);
            int opalCount = ProcessHandler.ReadData("crate opal count", crateAddress + 0x178, 1)[0];
            int firstCrateOpalAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x28AB7C), new int[] { 0x4AC, 0x0 });
            for (int i = 0; i < opalCount; i++)
            {
                int opalAddress = PointerCalculations.GetPointerAddress(crateAddress + 0x150 + (4 * i), 0x0);
                int opalIndex = (opalAddress - firstCrateOpalAddress) / 0x114;
                if (Program.HSync.SyncObjects["Opal"].GlobalObjectData[Program.HLevel.CurrentLevelId][i] == 5) return;
                ProcessHandler.WriteData(opalAddress + 0x78, BitConverter.GetBytes(1));
            }
        }
    }
}
