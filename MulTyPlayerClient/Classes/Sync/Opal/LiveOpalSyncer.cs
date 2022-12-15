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
            int crateOpalsInLevel = HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId];
            byte[] currentOpals = new byte[300];
            int address = HOpal.NonCrateOpalsAddress;
            byte[] buffer = new byte[1];
            int bytesRead = 0;
            for (int i = 0; i < 300 - crateOpalsInLevel; i++)
            {
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), address + 0x78 + (0x114 * i), buffer, 1, ref bytesRead);
                currentOpals[i] = buffer[0];
            }
            if (crateOpalsInLevel == 0) return currentOpals;

            address = HOpal.CrateOpalsAddress;
            for (int i = 0; i < crateOpalsInLevel; i++)
            {
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), address + 0x78 + (0x114 * i), buffer, 1, ref bytesRead);
                currentOpals[300 - crateOpalsInLevel + i] = buffer[0];
            }
            return currentOpals;
        }

        public override void Collect(int index)
        {
            if (HOpal.CurrentObjectData[index] >= 3) return;
            if (Program.HGameState.CheckMenuOrLoading()) return;
            int baseAddress;
            int crateOpalsInCurrentLevel = HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId];

            if (HLevel.CurrentLevelId == 10) baseAddress = HOpal.B3OpalsAddress;
            else if (index < (300 - HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId])) baseAddress = HOpal.NonCrateOpalsAddress;
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
