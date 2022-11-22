using System;
using System.Drawing;
using System.Net;

namespace MulTyPlayerClient
{
    internal class LiveOpalSyncer : LiveDataSyncer
    {
        readonly OpalHandler HOpal;
        static LevelHandler HLevel => Program.HLevel;

        public LiveOpalSyncer(OpalHandler HOpal)
        {
            this.HOpal = HOpal;
            HSyncObject = HOpal;
        }

        public override byte[] ReadData()
        {
            int crateOpalsInLevel = HOpal._crateOpalsPerLevel[HLevel.CurrentLevelId];
            byte[] currentOpals = new byte[300];
            int address = HLevel.CurrentLevelId == 10 ? HOpal.B3OpalsAddress : HOpal.NonCrateOpalsAddress;
            for (int i = 0; i < 300 - crateOpalsInLevel; i++)
            {
                currentOpals[i] = ProcessHandler.ReadData("opal read", address + 0x78 + (0x114 * i), 1)[0];
            }
            if (crateOpalsInLevel == 0) return currentOpals;

            address = HOpal.CrateOpalsAddress;
            for (int i = 0; i < crateOpalsInLevel; i++)
            {
                currentOpals[300 - crateOpalsInLevel + i] = ProcessHandler.ReadData("opal read", address + 0x78 + (0x114 * i), 1)[0];
            }
            return currentOpals;
        }

        public override void Collect(int index)
        {
            if (HOpal.CurrentObjectData[index] >= 3) return;
            if (Program.HGameState.CheckMenuOrLoading()) return;
            int baseAddress;
            int crateOpalsInCurrentLevel = HOpal._crateOpalsPerLevel[HLevel.CurrentLevelId];

            if (HLevel.CurrentLevelId == 10) baseAddress = HOpal.B3OpalsAddress;
            else if (index < (300 - HOpal._crateOpalsPerLevel[HLevel.CurrentLevelId])) baseAddress = HOpal.NonCrateOpalsAddress;
            else
            {
                int nonCrateOpalsInCurrentLevel = 300 - crateOpalsInCurrentLevel;
                ProcessHandler.WriteData(HOpal.CrateOpalsAddress + 0x78 + (0x114 * (index - nonCrateOpalsInCurrentLevel)), BitConverter.GetBytes(3));
                return;
            }
            ProcessHandler.WriteData(baseAddress + 0x78 + (0x114 * index), BitConverter.GetBytes(3));
        }

    }
}
