using System;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class LiveOpalSyncer : LiveDataSyncer
    {
        readonly OpalHandler HOpal;
        static LevelHandler HLevel => Client.HLevel;

        public LiveOpalSyncer(OpalHandler HOpal)
        {
            this.HOpal = HOpal;
            HSyncObject = HOpal;
        }

        public override byte[] ReadData()
        {
            int crateOpalsInLevel = HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId];
            byte[] currentOpals = new byte[300];
            int address = HOpal.NonCrateOpalsAddress;
            for (int i = 0; i < 300 - crateOpalsInLevel; i++)
            {
                currentOpals[i] = ProcessHandler.ReadData(address + 0x78 + (0x114 * i), 1, $"Checking state of non crate opals {i} / {300 - crateOpalsInLevel}")[0];
            }
            if (crateOpalsInLevel == 0) return currentOpals;

            address = HOpal.CrateOpalsAddress;
            for (int i = 0; i < crateOpalsInLevel; i++)
            {
                currentOpals[300 - crateOpalsInLevel + i] = ProcessHandler.ReadData(address + 0x78 + (0x114 * i), 1, $"Checking state of crate opals {i} / {crateOpalsInLevel}")[0];
            }
            return currentOpals;
        }

        public override void Collect(int index)
        {
            if (HOpal.CurrentObjectData[index] >= 3) return;
            if (Client.HGameState.CheckMenuOrLoading()) return;
            int baseAddress;
            int crateOpalsInCurrentLevel = HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId];

            if (HLevel.CurrentLevelId == 10) baseAddress = HOpal.CrateOpalsAddress;
            else if (index < (300 - HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId])) baseAddress = HOpal.NonCrateOpalsAddress;
            else
            {
                int nonCrateOpalsInCurrentLevel = 300 - crateOpalsInCurrentLevel;
                ProcessHandler.WriteData(HOpal.CrateOpalsAddress + 0x78 + (0x114 * (index - nonCrateOpalsInCurrentLevel)), BitConverter.GetBytes(3), $"Collecting opal {index}");
                return;
            }
            ProcessHandler.WriteData(baseAddress + 0x78 + (0x114 * index), BitConverter.GetBytes(3), $"Collecting opal {index}");
        }

    }
}
