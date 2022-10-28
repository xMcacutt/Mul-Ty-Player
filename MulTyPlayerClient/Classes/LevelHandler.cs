using MulTyPlayerClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class LevelHandler
    {
        IntPtr HProcess => ProcessHandler.HProcess;
        static KoalaHandler HKoala => Program.HKoala;
        static SyncHandler HSync => Program.HSync;

        public int CurrentLevelId { get; set; }
        public bool LoadedIntoNewLevelStuffDone;
        readonly int[] _objectiveCountOffsetsSnow = { 0x30, 0x54, 0x54, 0x6C };
        readonly int[] _objectiveCountOffsetsStump = { 0x30, 0x34, 0x54, 0x6C };

        public int[] MainStages = { 4, 5, 6, 8, 9, 10, 12, 13, 14 };

        public void DoLevelSetup()
        {
            HSync.ProtectLeaderboard();
            HSync.RequestSync();
            HKoala.SetCoordAddrs();
            if (!SettingsHandler.DoKoalaCollision)
            {
                HKoala.RemoveCollision();
            }
            if (CurrentLevelId == 9 || CurrentLevelId == 13) { ObjectiveCountSet(); }
            LoadedIntoNewLevelStuffDone = true;
        }

        public void GetCurrentLevel()
        {
            int bytesRead = 0;
            byte[] currentLevelBytes = new byte[4];
            ProcessHandler.ReadProcessMemory((int)HProcess, PointerCalculations.AddOffset(0x280594), currentLevelBytes, 4, ref bytesRead);
            CurrentLevelId = BitConverter.ToInt32(currentLevelBytes, 0);
        }

        public void ObjectiveCountSet() 
        {
            int[] objectiveCountOffsets = null;
            int bytesRead = 0;
            switch (CurrentLevelId)
            {
                case 9:
                    objectiveCountOffsets = _objectiveCountOffsetsSnow;
                    break;
                case 13:
                    objectiveCountOffsets = _objectiveCountOffsetsStump;
                    break;
            }
            byte[] objectiveCountBytes = new byte[2];
            //POTENTIALLY UNECESSARY READ
            int objectiveCounterAddr = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x0028C318), objectiveCountOffsets, 2);
            ProcessHandler.ReadProcessMemory((int)HProcess, objectiveCounterAddr, objectiveCountBytes, 2, ref bytesRead);
            if (BitConverter.ToInt16(objectiveCountBytes, 0) != 8)
            {
                int bytesWritten = 0;
                byte[] buffer = BitConverter.GetBytes((Int16)8);
                ProcessHandler.WriteProcessMemory((int)HProcess, objectiveCounterAddr, buffer, buffer.Length, ref bytesWritten);
            }
        }
    }
}
