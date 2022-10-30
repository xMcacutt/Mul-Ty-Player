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
        /*
        readonly int _levelOpalDataBaseAddr = 0x00288730;
        readonly int _levelOpalDataOffset = 0x1D1;
        */

        IntPtr HProcess => ProcessHandler.HProcess;
        static SyncHandler HSync => Program.HSync;
        static LevelHandler HLevel => Program.HLevel;

        static int[] _crateOpalsPerLevel = { 170, 102, 119, 0, 120, 60, 300, 0, 30, 170, 215 };
        readonly static int _opalsBaseAddress = 0x0028AB70; // +0xC for b3 opals and non crate opals. +0x0 for crate opals
        readonly static int[] _nonCrateOpalsPath = { 0x180, 0x150, 0x3C, 0x14C, 0x120, 0x88 };
        static public int NonCrateOpalsAddress;
        static public int CrateOpalsAddress;
        static public int B3OpalsAddress;

        public byte[] CurrentOpalData;
        public byte[] PreviousOpalData;
        public int OpalCount;
        public int PreviousOpalCount;
        static int _serverOpalCount;

        public OpalHandler()
        {
            PreviousOpalData = new byte[300];
            CurrentOpalData = new byte[300];
        }

        public static void SetMemAddrs()
        {
            NonCrateOpalsAddress = PointerCalculations.GetPointerAddressNegative(
                PointerCalculations.AddOffset(_opalsBaseAddress) + 0xC,
                _nonCrateOpalsPath,
                 0x6C9C);
            B3OpalsAddress = PointerCalculations.GetPointerAddress(
                PointerCalculations.AddOffset(_opalsBaseAddress) + 0xC,
                new int[] { 0x2B0, 0x78 });
            CrateOpalsAddress = PointerCalculations.GetPointerAddress(
                PointerCalculations.AddOffset(_opalsBaseAddress),
                0x78);
        }

        public void HandleClientUpdate(int opal, int level, int opalCountForLevel)
        {
            if (level == HLevel.CurrentLevelId)
            {
                _serverOpalCount = opalCountForLevel;
                WriteOpal(opal);
            }
        }

        public void CheckCount()
        {
            if (!HLevel.MainStages.Contains(HLevel.CurrentLevelId)) { return; }
            ReadOpalCount();
            if (PreviousOpalCount == OpalCount || OpalCount == 0) return;
            PreviousOpalCount = OpalCount;
            if (OpalCount == _serverOpalCount) return;
            CurrentOpalData = ReadCurrentOpals();
            for (int i = 0; i < 300; i++)
            {
                if (PreviousOpalData[i] < 4 && CurrentOpalData[i] > 3)
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
            ProcessHandler.ReadProcessMemory((int)HProcess, PointerCalculations.AddOffset(0x26547C), buffer, 4, ref bytesRead);
            OpalCount = BitConverter.ToInt32(buffer, 0);
        }

        byte[] ReadCurrentOpals()
        {
            byte[] currentOpals = new byte[300];
            int bytesRead = 0;
            byte[] buffer = new byte[1];
            if(HLevel.CurrentLevelId == 10)
            {
                for(int i = 0; i < 300; i++)
                {
                    ProcessHandler.ReadProcessMemory((int)HProcess, B3OpalsAddress + (0x114 * i), buffer, 1, ref bytesRead);
                    currentOpals[i] = buffer[0];
                }
                return currentOpals;
            }
            int crateOpalsInLevel = _crateOpalsPerLevel[HLevel.CurrentLevelId - 4];
            for(int i = 0; i < (300 - crateOpalsInLevel); i++)
            {
                ProcessHandler.ReadProcessMemory((int)HProcess, NonCrateOpalsAddress + (0x114 * i), buffer, 1, ref bytesRead);
                currentOpals[i] = buffer[0];
            }
            for(int i = 0; i < crateOpalsInLevel; i++)
            {
                ProcessHandler.ReadProcessMemory((int)HProcess, CrateOpalsAddress + (0x114 * i), buffer, 1, ref bytesRead);
                currentOpals[300 - crateOpalsInLevel + i] = buffer[0];
            }
            return currentOpals;
        }

        public void WriteCurrentOpalsSync(byte[] bytes)
        {
            for (int i = 0; i < 300; i++)
            {
                if (bytes[i] > 3)
                {
                    WriteOpal(i);
                }
            }
        }

        public void WriteOpal(int opalIndex)
        {
            if (Program.HGameState.CheckMenu()) { return; }
            if (HLevel.CurrentLevelId == 10)
            {
                ProcessHandler.WriteData(B3OpalsAddress + (0x114 * opalIndex), BitConverter.GetBytes(3));
                //ProcessHandler.WriteData(PointerCalculations.AddOffset(0x2888B0), BitConverter.GetBytes(_serverOpalCount));
                return;
            }
            int crateOpalsInLevel = _crateOpalsPerLevel[HLevel.CurrentLevelId - 4];
            if (opalIndex < (300 - crateOpalsInLevel))
            {
                ProcessHandler.WriteData(NonCrateOpalsAddress + (0x114 * opalIndex), BitConverter.GetBytes(3));
                //ProcessHandler.WriteData(PointerCalculations.AddOffset(0x2888B0), BitConverter.GetBytes(_serverOpalCount));
                return;
            }
            ProcessHandler.WriteData(CrateOpalsAddress + (0x114 * (opalIndex - (300 - crateOpalsInLevel))), BitConverter.GetBytes(3));

            // ProcessHandler.WriteData(PointerCalculations.AddOffset(0x2888B0), BitConverter.GetBytes(_serverOpalCount));
        }
    }
}
