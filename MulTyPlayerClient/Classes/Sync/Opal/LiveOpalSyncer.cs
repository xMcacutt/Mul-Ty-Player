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
        }

        private int[] SelectOpalType()
        {
            int size = 300;
            int address;
            switch (HLevel.CurrentLevelId)
            {
                case 0:
                    address = HOpal.RainbowScaleAddress;
                    size = 25;
                    break;
                case 10:
                    address = HOpal.B3OpalsAddress;
                    break;
                default:
                    address = HOpal.NonCrateOpalsAddress;
                    break;
            }
            int[] result = { address, size };
            return result;
        }

        public override byte[] ReadData()
        {
            int crateOpalsInLevel = HOpal._crateOpalsPerLevel[HLevel.CurrentLevelId];

            int[] opalTypeData = SelectOpalType();
            int address = opalTypeData[0];
            int size = opalTypeData[1];

            byte[] currentOpals = new byte[size];
            for (int i = 0; i < size - crateOpalsInLevel; i++)
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
            if (HLevel.CurrentLevelId == 0) baseAddress = HOpal.RainbowScaleAddress;
            else if (HLevel.CurrentLevelId == 10) baseAddress = HOpal.B3OpalsAddress;
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
