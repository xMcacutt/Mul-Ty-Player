using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes
{
    internal class OpalHandler
    {
        IntPtr HProcess => ProcessHandler.HProcess;
        static SyncHandler HSync => Program.HSync;
        static LevelHandler HLevel => Program.HLevel;

        readonly int _currentOpalDataBaseAddr = 0x0028AB7C;
        readonly int[] _currentOpalDataOffsets = { 0x180, 0x150, 0x3C, 0x14C, 0x120, 0x88 };
        readonly int _levelOpalDataBaseAddr = 0x00288730;
        readonly int _levelOpalDataOffset = 0x1D1;

        static public int CurrentOpalsDataAddress;
        static public int OpalSaveDataAddress;

        public byte[] CurrentOpalData;
        public byte[] PreviousOpalData;
        public Dictionary<int, byte[]> LevelOpalData;
        public int OpalCount;
        public int PreviousOpalCount;
        static int _serverOpalCount;

        public OpalHandler()
        {
            CurrentOpalsDataAddress = PointerCalculations.GetPointerAddressNegative(
                PointerCalculations.AddOffset(_currentOpalDataBaseAddr), 
                _currentOpalDataOffsets, 
                0x6B58);
            OpalSaveDataAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(_levelOpalDataBaseAddr), _levelOpalDataOffset);

            LevelOpalData = new Dictionary<int, byte[]>
            {
                { 4, ReadOpalSaveData(0) },
                { 5, ReadOpalSaveData(1) },
                { 6, ReadOpalSaveData(2) },
                { 8, ReadOpalSaveData(4) },
                { 9, ReadOpalSaveData(5) },
                { 10, ReadOpalSaveData(6) },
                { 12, ReadOpalSaveData(8) },
                { 13, ReadOpalSaveData(9) },
                { 14, ReadOpalSaveData(10) }
            };

            PreviousOpalData = new byte[300];
            CurrentOpalData = new byte[300];
        }

        public void CheckCount()
        {
            ReadOpalCount();
            if (PreviousOpalCount == OpalCount) return;  
            PreviousOpalCount = OpalCount;
            if (OpalCount == _serverOpalCount) return;
            CurrentOpalData = ReadCurrentOpals();
            for (int i = 0; i < 300; i++)
            {
                if (PreviousOpalData[i] != CurrentOpalData[i])
                {
                    HSync.UpdateServerData(HLevel.CurrentLevelId, BitConverter.GetBytes(i), "Opal");
                    PreviousOpalData[i] = CurrentOpalData[i];
                    i = 300;
                }
            }
        }

        public void WriteCurrentOpalsSync()
        {
            byte[] collected = new byte[1] { 3 };
            for (int i = 0; i < 300; i++)
            {
                if (CurrentOpalData[i] == 1)
                {
                    ProcessHandler.WriteData(CurrentOpalsDataAddress + 0x114 * (i - 4), collected);
                }
            }
        }

        [MessageHandler((ushort)MessageID.OpalCollected)]
        static void UpdateClientWithOpalData(Message message)
        {
            int level = message.GetInt();
            int opal = message.GetInt();
            byte[] saveDataBytes = message.GetBytes();
            int opalCountForLevel = message.GetInt();

            if (level == HLevel.CurrentLevelId)
            {
                _serverOpalCount = opalCountForLevel;
                ProcessHandler.WriteData(CurrentOpalsDataAddress + 0x114 * opal, BitConverter.GetBytes(3));
            }
            ProcessHandler.WriteData(OpalSaveDataAddress + 70 * (level - 4), saveDataBytes);
        }

        void ReadOpalCount()
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            ProcessHandler.ReadProcessMemory((int)HProcess, PointerCalculations.AddOffset(0x2888B0), buffer, 4, ref bytesRead);
            OpalCount = BitConverter.ToInt32(buffer, 0);
        }

        byte[] ReadOpalSaveData(int levelIndex)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[38];
            ProcessHandler.ReadProcessMemory((int)HProcess, OpalSaveDataAddress + (0x70 * levelIndex), buffer, 38, ref bytesRead);
            return buffer;
        }

        byte[] ReadCurrentOpals()
        {
            byte[] currentOpals = new byte[300];
            int bytesRead = 0;
            byte[] buffer = new byte[1];
            for(int i = 0; i < 300; i++)
            {
                ProcessHandler.ReadProcessMemory((int)HProcess, CurrentOpalsDataAddress, buffer, 1, ref bytesRead);
                currentOpals[i] = buffer[0];
            }
            return currentOpals;
        }

    }
}
