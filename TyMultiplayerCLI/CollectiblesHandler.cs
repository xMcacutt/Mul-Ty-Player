using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class CollectiblesHandler
    {
        static HeroHandler HeroHandler => Program.HeroHandler;

        readonly int TE_COUNTER_ADDRESS = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0xD);
        readonly int COG_COUNTER_ADDRESS = PointerCalculations.AddOffset(0x265260);
        readonly int BILBY_COUNTER_ADDRESS = PointerCalculations.AddOffset(0x2651AC);
        int[] _counterAddresses;
        public int[] CollectibleCounts;
        public int[] PreviousCollectibleCounts;
        readonly static int LEVEL_DATA_START_ADDRESS = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0x1F8);
        readonly static int ATTRIBUTE_DATA_START_ADDRESS = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x00288730), 0xAB4);

        public static Dictionary<int, byte[]> LevelData;
        public static byte[] AttributeData;
        public byte[] PreviousAttributeData;

        static IntPtr HProcess => ProcessHandler.HProcess;

        public CollectiblesHandler()
        {
            _counterAddresses = new int[3];
            _counterAddresses[0] = TE_COUNTER_ADDRESS;
            _counterAddresses[1] = COG_COUNTER_ADDRESS; 
            _counterAddresses[2] = BILBY_COUNTER_ADDRESS;
            LevelData = new Dictionary<int, byte[]>();

            LevelData.Add(4, ReadLevelData(0));
            LevelData.Add(5, ReadLevelData(1));
            LevelData.Add(6, ReadLevelData(2));
            LevelData.Add(8, ReadLevelData(4));
            LevelData.Add(9, ReadLevelData(5));
            LevelData.Add(10, ReadLevelData(6));
            LevelData.Add(12, ReadLevelData(8));
            LevelData.Add(13, ReadLevelData(9));
            LevelData.Add(14, ReadLevelData(10));

            AttributeData = new byte[26];
            PreviousAttributeData = new byte[26];
            CollectibleCounts = new int[3];
            PreviousCollectibleCounts = new int[3];
            AttributeData = ReadAttributeData();
        }

        //POSSIBLY UNNECESSARY
        public void ReadCounts()
        {
            int bytesRead = 0;
            for(int i = 0; i < 3; i++)
            {
                byte[] buffer = new byte[1];
                ProcessHandler.ReadProcessMemory((int)HProcess, _counterAddresses[i], buffer, 1, ref bytesRead);
                CollectibleCounts[i] = buffer[0];
            }
        }

        public void CheckCounts()
        {
            ReadCounts();
            LevelData[HeroHandler.CurrentLevelId] = ReadLevelData(HeroHandler.CurrentLevelId - 4);
            AttributeData = ReadAttributeData();
      //      Console.WriteLine($"Current TE Count = {CollectibleCounts[0]}\nPrevious TE Count = {PreviousCollectibleCounts[0]}");
            if (PreviousCollectibleCounts[0] != CollectibleCounts[0] || PreviousCollectibleCounts[1] != CollectibleCounts[1] || PreviousCollectibleCounts[2] != CollectibleCounts[2])
            {
                Console.WriteLine("Goodbye");
                PreviousCollectibleCounts = CollectibleCounts;
                UpdateServerData(HeroHandler.CurrentLevelId, LevelData[HeroHandler.CurrentLevelId], "Collectible");
            }
            if(!Enumerable.SequenceEqual(PreviousAttributeData, AttributeData))
            {
                PreviousAttributeData = AttributeData;
                UpdateServerData(0, AttributeData, "Attribute");
            }
        }
        //
        public static void WriteData(int address, byte[] bytes)
        {
            int bytesWritten = 0;
            ProcessHandler.WriteProcessMemory((int)HProcess, address, bytes, bytes.Length, ref bytesWritten);
        }


        public byte[] ReadAttributeData()
        {
            int bytesRead = 0;
            byte[] buffer = new byte[23];
            ProcessHandler.ReadProcessMemory((int)HProcess, ATTRIBUTE_DATA_START_ADDRESS, buffer, 26, ref bytesRead);
            return buffer;
        }

        public byte[] ReadLevelData(int levelDataIndex)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[23];
            ProcessHandler.ReadProcessMemory((int)HProcess, LEVEL_DATA_START_ADDRESS + (0x70 * levelDataIndex), buffer, 23, ref bytesRead);
            return buffer;
        }

        public void UpdateServerData(int level, byte[] data, string type)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ServerDataUpdate);
            message.AddBytes(data);
            message.AddInt(level);
            message.AddString(type);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ClientAttributeDataUpdate)]
        public static void UpdateClientWithAttr(Message message)
        {
            AttributeData = message.GetBytes();
            WriteData(ATTRIBUTE_DATA_START_ADDRESS, AttributeData);
        }

        [MessageHandler((ushort)MessageID.ClientLevelDataUpdate)]
        public static void UpdateClientWithLevelData(Message message)
        {
            int level = message.GetInt();
            LevelData[level] = message.GetBytes();
            bool[] doSync = message.GetBools();
            Console.WriteLine("hello");
            if (doSync[0]) { WriteData(LEVEL_DATA_START_ADDRESS + (0x70 * (level - 4)), LevelData[level].Take(8).ToArray()); }
            if (doSync[1]) { WriteData(LEVEL_DATA_START_ADDRESS + (0x70 * (level - 4)) + 0x8, LevelData[level].Skip(8).Take(10).ToArray()); }
            if (doSync[2]) { WriteData(LEVEL_DATA_START_ADDRESS + (0x70 * (level - 4)) + 0x18, LevelData[level].Skip(18).Take(5).ToArray()); }
        }
    }
}
