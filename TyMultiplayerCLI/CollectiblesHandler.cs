using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class CollectiblesHandler
    {
        readonly int TE_COUNTER_ADDRESS = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0xD);
        readonly int COG_COUNTER_ADDRESS = PointerCalculations.AddOffset(0x265260);
        readonly int BILBY_COUNTER_ADDRESS = PointerCalculations.AddOffset(0x2651AC);
        readonly int LEVEL_DATA_START_ADDRESS = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0x1F8);
        readonly int ATTRIBUTE_DATA_START_ADDRESS = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0xAB4);

        public byte[][] LevelData;
        public byte[] AttributeData;

        IntPtr HProcess => ProcessHandler.HProcess;

        public CollectiblesHandler()
        {
            LevelData[4] = ReadLevelData(0);
            LevelData[5]= ReadLevelData(1);
            LevelData[6] = ReadLevelData(2);
            LevelData[8] = ReadLevelData(4);
            LevelData[9] = ReadLevelData(5);
            LevelData[10] = ReadLevelData(6);
            LevelData[12] = ReadLevelData(8);
            LevelData[13] = ReadLevelData(9);
            LevelData[14] = ReadLevelData(10);

            AttributeData = ReadAttributeData();
        }

        public byte[] ReadAttributeData()
        {
            int bytesRead = 0;
            byte[] buffer = new byte[23];
            ProcessHandler.ReadProcessMemory((int)HProcess, ATTRIBUTE_DATA_START_ADDRESS, buffer, 23, ref bytesRead);
            return buffer;
        }

        public byte[] ReadLevelData(int levelId)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[23];
            ProcessHandler.ReadProcessMemory((int)HProcess, LEVEL_DATA_START_ADDRESS + (0x70 * levelId), buffer, 23, ref bytesRead);
            return buffer;
        }
    }
}
