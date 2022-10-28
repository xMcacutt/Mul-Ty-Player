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
        readonly int _levelOpalDataBaseAddr = 0x00288730;
        readonly int _levelOpalDataOffset = 0x1D1;
        IntPtr HProcess => ProcessHandler.HProcess;
        static SyncHandler HSync => Program.HSync;
        static LevelHandler HLevel => Program.HLevel;

        readonly int _currentOpalDataBaseAddr = 0x0028AB7C;
        readonly int[] _currentOpalDataOffsets = { 0x180, 0x150, 0x3C, 0x14C, 0x120, 0x88 };
        static public int CurrentOpalsDataAddress;
        static public int OpalSaveDataAddress;

        public byte[] CurrentOpalData;
        public byte[] PreviousOpalData;
        public int OpalCount;
        public int PreviousOpalCount;
        static int _serverOpalCount;

        public OpalHandler()
        {
            CurrentOpalsDataAddress = PointerCalculations.GetPointerAddressNegative(
                PointerCalculations.AddOffset(_currentOpalDataBaseAddr), 
                _currentOpalDataOffsets, 
                0x6C9C);
            PreviousOpalData = new byte[300];
            CurrentOpalData = new byte[300];
        }

        public void HandleClientUpdate(int opal, int level, int opalCountForLevel)
        {
            if (level == HLevel.CurrentLevelId)
            {
                _serverOpalCount = opalCountForLevel;
                ProcessHandler.WriteData(CurrentOpalsDataAddress + 0x114 * opal, BitConverter.GetBytes(3));
            }
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
                if ((PreviousOpalData[i] == 0 || PreviousOpalData[i] == 2)  && CurrentOpalData[i] == 5)
                {
                    PreviousOpalData[i] = CurrentOpalData[i];
                    HSync.UpdateServerData(HLevel.CurrentLevelId, BitConverter.GetBytes(i), "Opal");
                }
            }
        }

        void ReadOpalCount()
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            ProcessHandler.ReadProcessMemory((int)HProcess, PointerCalculations.AddOffset(0x2888B0), buffer, 4, ref bytesRead);
            OpalCount = BitConverter.ToInt32(buffer, 0);
        }

        byte[] ReadCurrentOpals()
        {
            byte[] currentOpals = new byte[300];
            int bytesRead = 0;
            byte[] buffer = new byte[1];
            for(int i = 0; i < 300; i++)
            {
                ProcessHandler.ReadProcessMemory((int)HProcess, CurrentOpalsDataAddress + (0x114 * i), buffer, 1, ref bytesRead);
                currentOpals[i] = buffer[0];
            }
            return currentOpals;
        }

        public void WriteCurrentOpalsSync(byte[] bytes)
        {
            byte[] collected = new byte[1] { 3 };
            for (int i = 0; i < 300; i++)
            {
                if (bytes[i] == 5)
                {
                    ProcessHandler.WriteData(CurrentOpalsDataAddress + 0x114 * i, collected);
                }
            }
        }
    }
}
