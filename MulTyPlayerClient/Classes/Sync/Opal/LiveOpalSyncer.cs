using System;
using System.Drawing;
using System.Linq;
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
            // Get the number of crate opals in the current level
            int crateOpalsInLevel = HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId];

            // Create an array to store the current opals
            byte[] currentOpals = new byte[300];

            // Read the non-crate opals from memory
            int address = HOpal.NonCrateOpalsAddress;
            for (int i = 0; i < 300 - crateOpalsInLevel; i++)
            {
                ProcessHandler.TryRead(address + 0x78 + (0x114 * i), out currentOpals[i], false);
            }

            // If there are no crate opals in the level, return the current opals array
            if (crateOpalsInLevel == 0) return currentOpals;

            // Read the crate opals from memory and add them to the current opals array
            address = HOpal.CrateOpalsAddress;
            for (int i = 0; i < crateOpalsInLevel; i++)
            {
                ProcessHandler.TryRead(address + 0x78 + (0x114 * i), out currentOpals[300 - crateOpalsInLevel + i], false);
            }

            return currentOpals;
        }

        private void ReadSpecificOpalState(int index)
        {
            int crateOpalsInLevel = HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId];
            if(index > (300 - crateOpalsInLevel))
            {

            }
        }

        public override void Collect(int index)
        {
            if (HOpal.CurrentObjectData[index] >= 3) return;
            if (Client.HGameState.CheckMenuOrLoading()) return;
            int baseAddress;
            int crateOpalsInCurrentLevel = HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId];
            int address;
            if (HLevel.CurrentLevelId == 10) baseAddress = HOpal.CrateOpalsAddress;
            else if (index < (300 - HOpal.CrateOpalsPerLevel[HLevel.CurrentLevelId])) baseAddress = HOpal.NonCrateOpalsAddress;
            else
            {
                int nonCrateOpalsInCurrentLevel = 300 - crateOpalsInCurrentLevel;
                address = HOpal.CrateOpalsAddress + 0x78 + (0x114 * (index - nonCrateOpalsInCurrentLevel));
                ProcessHandler.TryRead(address, out HOpal.CurrentObjectData[index], false);
                if (HOpal.CurrentObjectData[index] >= 3) return;
                ProcessHandler.WriteData(HOpal.CrateOpalsAddress + 0x78 + (0x114 * (index - nonCrateOpalsInCurrentLevel)), BitConverter.GetBytes(3), $"Collecting opal {index}");
                return;
            }
            address = baseAddress + 0x78 + (0x114 * index);
            ProcessHandler.TryRead(address, out HOpal.CurrentObjectData[index], false);
            if (HOpal.CurrentObjectData[index] >= 3) return;
            ProcessHandler.WriteData(address, BitConverter.GetBytes(3), $"Collecting opal {index}");
        }

    }
}
