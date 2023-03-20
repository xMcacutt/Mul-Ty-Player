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

        public async override void Collect(int index)
        {
            int crateAddress = HSyncObject.LiveObjectAddress + (index * ObjectLength);
            await ProcessHandler.WriteDataAsync(crateAddress + 0x48, new byte[] {0});
            await ProcessHandler.WriteDataAsync(crateAddress + 0x114, new byte[] {0});
            int opalCount = (await ProcessHandler.ReadDataAsync(crateAddress + 0x178, 1))[0];
            for (int i = 0; i < opalCount; i++)
            {
                int opalAddress = await PointerCalculations.GetPointerAddress(crateAddress + 0x150 + (4 * i), new int[] { 0x0 });
                if ((await ProcessHandler.ReadDataAsync(opalAddress + 0x78, 1))[0] != 5)
                {
                    await ProcessHandler.WriteDataAsync(opalAddress + 0x78, BitConverter.GetBytes(1));
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

        public async override Task<byte[]> ReadData()
        {
            byte[] currentData = new byte[SyncHandler.HCrate.CratesPerLevel[Client.HLevel.CurrentLevelId]];
            int address = HSyncObject.LiveObjectAddress;
            for (int i = 0; i < currentData.Length; i++)
            {
                currentData[i] = (await ProcessHandler.ReadDataAsync(address + StateOffset + (ObjectLength * i), 1))[0];
            }
            return currentData;
        }
    }
}
