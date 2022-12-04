using MulTyPlayerClient;
using Riptide;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


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
            HKoala.SetCoordAddrs();
            HSync.SetMemAddrs();
            HSync.RequestSync();
            if (!SettingsHandler.DoKoalaCollision) HKoala.RemoveCollision();
            if (CurrentLevelId == 9 || CurrentLevelId == 13) ObjectiveCountSet();
            if (CurrentLevelId == 0 && SettingsHandler.DoPortalSyncing) 
            { 
                HSync.SyncObjects["Portal"].Sync(0, (HSync.SyncObjects["Portal"] as PortalHandler).PortalsActive.Values.ToArray(), new byte[] { 0 }); 
            }
            LoadedIntoNewLevelStuffDone = true;
        }

        public void GetCurrentLevel()
        {
            byte[] buffer = ProcessHandler.ReadData("current level", PointerCalculations.AddOffset(0x280594), 4);
            CurrentLevelId = BitConverter.ToInt32(buffer, 0);
        }

        public void ObjectiveCountSet() 
        {
            int[] objectiveCountOffsets = null;
            switch (CurrentLevelId)
            {
                case 9:
                    objectiveCountOffsets = _objectiveCountOffsetsSnow;
                    break;
                case 13:
                    objectiveCountOffsets = _objectiveCountOffsetsStump;
                    break;
            }
            int objectiveCounterAddr = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x0028C318), objectiveCountOffsets, 2);
            byte[] buffer = ProcessHandler.ReadData("objective count", objectiveCounterAddr, 2);
            if (BitConverter.ToInt16(buffer, 0) != 8) ProcessHandler.WriteData(objectiveCounterAddr, BitConverter.GetBytes(8));
        }
    }
}
