using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;

namespace MulTyPlayerClient
{
    internal class CollectiblesHandler
    {
        static IntPtr HProcess => ProcessHandler.HProcess;
        static LevelHandler HLevel => Program.HLevel;
        static SyncHandler HSync => Program.HSync;

        int _currentLevel => HLevel.CurrentLevelId;

        public readonly int TECounterAddress;
        public readonly int CogCounterAddress;
        public readonly int BilbyCounterAddress;
        public readonly int LevelDataStartAddress;
        int[] _counterAddresses;

        public int[] CollectibleCounts;
        public int[] PreviousCollectibleCounts;

        public Dictionary<int, byte[]> LevelData;

        public CollectiblesHandler()
        {
            LevelDataStartAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0x1F8);
            TECounterAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0xD);
            CogCounterAddress = PointerCalculations.AddOffset(0x265260);
            BilbyCounterAddress = PointerCalculations.AddOffset(0x2651AC);

            _counterAddresses = new int[3];
            _counterAddresses[0] = TECounterAddress;
            _counterAddresses[1] = CogCounterAddress;
            _counterAddresses[2] = BilbyCounterAddress;
            LevelData = new Dictionary<int, byte[]>
            {
                { 4, ReadLevelData(0) },
                { 5, ReadLevelData(1) },
                { 6, ReadLevelData(2) },
                { 8, ReadLevelData(4) },
                { 9, ReadLevelData(5) },
                { 10, ReadLevelData(6) },
                { 12, ReadLevelData(8) },
                { 13, ReadLevelData(9) },
                { 14, ReadLevelData(10) }
            };

            CollectibleCounts = new int[3];
            PreviousCollectibleCounts = new int[3];
        }

        public void ReadCounts()
        {
            int bytesRead = 0;
            for (int i = 0; i < 3; i++)
            {
                byte[] buffer = new byte[1];
                ProcessHandler.ReadProcessMemory((int)HProcess, _counterAddresses[i], buffer, 1, ref bytesRead);
                CollectibleCounts[i] = buffer[0];
            }
        }

        public void CheckCountsChanged()
        {
            ReadCounts();
            if (!Enumerable.SequenceEqual(PreviousCollectibleCounts, CollectibleCounts))
            {
                if (LevelData.ContainsKey(HLevel.CurrentLevelId))
                {
                    LevelData[_currentLevel] = ReadLevelData(_currentLevel - 4);
                    HSync.UpdateServerData(_currentLevel, LevelData[_currentLevel], "Collectible");
                }
                PreviousCollectibleCounts[0] = CollectibleCounts[0];
                PreviousCollectibleCounts[1] = CollectibleCounts[1];
                PreviousCollectibleCounts[2] = CollectibleCounts[2];
            }
        }

        public byte[] ReadLevelData(int levelDataIndex)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[23];
            ProcessHandler.ReadProcessMemory((int)HProcess, LevelDataStartAddress + (0x70 * levelDataIndex), buffer, 23, ref bytesRead);
            return buffer;
        }
    }
}
