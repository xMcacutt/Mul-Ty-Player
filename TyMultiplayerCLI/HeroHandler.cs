using System;
using System.Diagnostics;
using TyMultiplayerCLI;

public class HeroHandler
{
    
    readonly int[] TY_POS_ROT_OFFSETS = { 0x270B78, 0x270B7C, 0x270B80, 0x271C20 };
    int[] TY_POS_ROT_ADS;
    readonly int[] BP_POS_ROT_OFFSETS = { 0x254268, 0x25426C, 0x254270, 0x2545F4, };
    int[] BP_POS_ROT_ADS;
    const int LEVEL_ID_OFFSET = 0x280594;
    const int LOADING_OFFSET = 0x286555;
    const int OBJECTIVE_COUNT_BASE_OFFSET = 0x0028C318;
    int[] objectiveCountOffsetsSnow = { 0x30, 0x54, 0x54, 0x6C };
    int[] objectiveCountOffsetsStump = { 0x30, 0x34, 0x54, 0x6C };
    int LEVEL_ID_AD;
    int LOADING_AD;
    public float[] currentPosRot;
    public int currentLevelID;
    IntPtr hProcess => ProcessHandler.hProcess;
    PointerCalculations ptrCalc => Program.ptrCalc;


    public HeroHandler()
    {
        currentPosRot = new float[4];
    }

    public void SetMemoryAddresses()
    {
        //SET MEMORY ADDRESSES USING BASE ADDRESS OF TY APP AND ADDING OFFSETS
        TY_POS_ROT_ADS = new []
        { 
            ptrCalc.AddOffset(TY_POS_ROT_OFFSETS[0]), 
            ptrCalc.AddOffset(TY_POS_ROT_OFFSETS[1]), 
            ptrCalc.AddOffset(TY_POS_ROT_OFFSETS[2]), 
            ptrCalc.AddOffset(TY_POS_ROT_OFFSETS[3]) 
        };
        BP_POS_ROT_ADS = new[]
        { 
            ptrCalc.AddOffset(BP_POS_ROT_OFFSETS[0]), 
            ptrCalc.AddOffset(BP_POS_ROT_OFFSETS[1]), 
            ptrCalc.AddOffset(BP_POS_ROT_OFFSETS[2]),
            ptrCalc.AddOffset(BP_POS_ROT_OFFSETS[3]) 
        };
        LEVEL_ID_AD = ptrCalc.AddOffset(LEVEL_ID_OFFSET);
        LOADING_AD = ptrCalc.AddOffset(LOADING_OFFSET);
    }

    public void GetTyPos()
    {
        GetCurrentLevel();
        int[] tempInts = currentLevelID == 10 ? BP_POS_ROT_ADS : TY_POS_ROT_ADS;
        int bytesRead = 0;
        for (int i = 0; i < 4; i++)
        {
            byte[] buffer = new byte[4];
            ProcessHandler.ReadProcessMemory((int)hProcess, tempInts[i], buffer, 4, ref bytesRead);
            currentPosRot[i] = BitConverter.ToSingle(buffer, 0);
        }
    }

    public bool CheckLoading()
    {
        int bytesRead = 0;
        byte[] loading = new byte[1];
        ProcessHandler.ReadProcessMemory((int)hProcess, LOADING_AD, loading, 1, ref bytesRead);
        return BitConverter.ToBoolean(loading, 0);
    }

    public void GetCurrentLevel()
    {
        int bytesRead = 0;
        byte[] currentLevelBytes = new byte[4];
        ProcessHandler.ReadProcessMemory((int)hProcess, LEVEL_ID_AD, currentLevelBytes, 4, ref bytesRead);
        currentLevelID = BitConverter.ToInt32(currentLevelBytes, 0);

        int[] objectiveCountOffsets = null;

        switch (currentLevelID)
        {
            case 9:
                objectiveCountOffsets = objectiveCountOffsetsSnow;
                break;
            case 13:
                objectiveCountOffsets = objectiveCountOffsetsStump;
                break;
        }

        if (objectiveCountOffsets == null) { return; }

        byte[] objectiveCountBytes = new byte[2];
        int objectiveCounterAddr = ptrCalc.GetPointerAddress(ptrCalc.AddOffset(OBJECTIVE_COUNT_BASE_OFFSET), objectiveCountOffsets, 2);
        ProcessHandler.ReadProcessMemory((int)hProcess, objectiveCounterAddr, objectiveCountBytes, 2, ref bytesRead);
        if (BitConverter.ToInt16(objectiveCountBytes, 0) != 8 && !CheckLoading())
        {
            int bytesWritten = 0;
            byte[] buffer = BitConverter.GetBytes((Int16)8);
            ProcessHandler.WriteProcessMemory((int)hProcess, objectiveCounterAddr, buffer, buffer.Length, ref bytesWritten);
        }
    }



}

