using System;

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

        public override byte[] ReadData()
        {
            int address;
            int size = 300;
            int crateOpalsInLevel = HOpal._crateOpalsPerLevel[HLevel.CurrentLevelId];
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
            byte[] currentOpals = new byte[size];
            for(int i = 0; i < size - crateOpalsInLevel; i++)
            {
                currentOpals[i] = ProcessHandler.ReadData("opal read", address + 0x78 + (0x114 * i), 1)[0];
            }
            if(crateOpalsInLevel != 0)
            {
                address = HOpal.CrateOpalsAddress;
                for(int i = 0; i < crateOpalsInLevel; i++)
                {
                    currentOpals[300 - crateOpalsInLevel + i] = ProcessHandler.ReadData("opal read", address + 0x78 + (0x114 * i), 1)[0];
                }
            }
            return currentOpals;
        }

        public override void Collect(int index)
        {
            if (HOpal.CurrentObjectData[index] >= 3) return;
            if (Program.HGameState.CheckMenuOrLoading()) return;
            int baseAddress;
            if (HLevel.CurrentLevelId == 0) baseAddress = HOpal.RainbowScaleAddress;
            else if (HLevel.CurrentLevelId == 10) baseAddress = HOpal.B3OpalsAddress;
            else if (index < (300 - HOpal._crateOpalsPerLevel[HLevel.CurrentLevelId])) baseAddress = HOpal.NonCrateOpalsAddress;
            else baseAddress = HOpal.CrateOpalsAddress;
            ProcessHandler.WriteData(baseAddress + 0x78 + (114 * index), BitConverter.GetBytes(3));
        }

    }
}
