using System;
using System.Diagnostics;

namespace MulTyPlayerClient
{
    public class HeroHandler
    { 
        readonly int[] TY_POS_ROT_OFFSETS = { 0x270B78, 0x270B7C, 0x270B80, 0x271C20 };
        int[] TY_POS_ROT_ADS;
        readonly int[] BP_POS_ROT_OFFSETS = { 0x254268, 0x25426C, 0x254270, 0x2545F4, };
        int[] BP_POS_ROT_ADS;
        const int LEVEL_ID_OFFSET = 0x280594;
        const int LOADING_OFFSET = 0x286555;
        const int MAIN_MENU_OFFSET = 0x286640; //ZERO WHEN ON MAIN MENU
        const int OBJECTIVE_COUNT_BASE_OFFSET = 0x0028C318;
        readonly int[] _objectiveCountOffsetsSnow = { 0x30, 0x54, 0x54, 0x6C };
        readonly int[] _objectiveCountOffsetsStump = { 0x30, 0x34, 0x54, 0x6C };
        int LEVEL_ID_AD;
        int LOADING_AD;
        int MAIN_MENU_AD;
        public float[] CurrentPosRot { get; set; }
        public int CurrentLevelId { get; set; }
        public int PreviousLevelID { get; set; }
        IntPtr HProcess => ProcessHandler.HProcess;

        public HeroHandler()
        {
            PreviousLevelID = 99;
            CurrentPosRot = new float[4];
        }

        public void SetMemoryAddresses()
        {
            //SET MEMORY ADDRESSES USING BASE ADDRESS OF TY APP AND ADDING OFFSETS
            TY_POS_ROT_ADS = new[]
            {
                PointerCalculations.AddOffset(TY_POS_ROT_OFFSETS[0]),
                PointerCalculations.AddOffset(TY_POS_ROT_OFFSETS[1]),
                PointerCalculations.AddOffset(TY_POS_ROT_OFFSETS[2]),
                PointerCalculations.AddOffset(TY_POS_ROT_OFFSETS[3])
            };
            BP_POS_ROT_ADS = new[]
            {
                PointerCalculations.AddOffset(BP_POS_ROT_OFFSETS[0]),
                PointerCalculations.AddOffset(BP_POS_ROT_OFFSETS[1]),
                PointerCalculations.AddOffset(BP_POS_ROT_OFFSETS[2]),
                PointerCalculations.AddOffset(BP_POS_ROT_OFFSETS[3])
            };
            LEVEL_ID_AD = PointerCalculations.AddOffset(LEVEL_ID_OFFSET);
            LOADING_AD = PointerCalculations.AddOffset(LOADING_OFFSET);
            MAIN_MENU_AD = PointerCalculations.AddOffset(MAIN_MENU_OFFSET);
        }

        public void GetTyPos()
        {
            int[] tempInts = CurrentLevelId == 10 ? BP_POS_ROT_ADS : TY_POS_ROT_ADS;
            int bytesRead = 0;
            for (int i = 0; i < 4; i++)
            {
                byte[] buffer = new byte[4];
                ProcessHandler.ReadProcessMemory((int)HProcess, tempInts[i], buffer, 4, ref bytesRead);
                CurrentPosRot[i] = BitConverter.ToSingle(buffer, 0);
            }
        }

        public bool CheckLoading()
        {
            int bytesRead = 0;
            byte[] loading = new byte[1];
            ProcessHandler.ReadProcessMemory((int)HProcess, LOADING_AD, loading, 1, ref bytesRead);
            return BitConverter.ToBoolean(loading, 0);
        }

        public bool CheckMenu()
        {
            int bytesRead = 0;
            byte[] menu = new byte[1];
            ProcessHandler.ReadProcessMemory((int)HProcess, MAIN_MENU_AD, menu, 1, ref bytesRead);
            return menu[0] == 0;
        }

        public void GetCurrentLevel()
        {
            int bytesRead = 0;
            byte[] currentLevelBytes = new byte[4];
            ProcessHandler.ReadProcessMemory((int)HProcess, LEVEL_ID_AD, currentLevelBytes, 4, ref bytesRead);
            CurrentLevelId = BitConverter.ToInt32(currentLevelBytes, 0);

            while (CheckLoading()) ;

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

            if (objectiveCountOffsets == null) { return; }

            byte[] objectiveCountBytes = new byte[2];
            int objectiveCounterAddr = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(OBJECTIVE_COUNT_BASE_OFFSET), objectiveCountOffsets, 2);
            ProcessHandler.ReadProcessMemory((int)HProcess, objectiveCounterAddr, objectiveCountBytes, 2, ref bytesRead);
            if (BitConverter.ToInt16(objectiveCountBytes, 0) != 8 && !CheckLoading())
            {
                int bytesWritten = 0;
                byte[] buffer = BitConverter.GetBytes((Int16)8);
                ProcessHandler.WriteProcessMemory((int)HProcess, objectiveCounterAddr, buffer, buffer.Length, ref bytesWritten);
            }
        }



    }
}