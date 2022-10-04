using System;

using System.Diagnostics;
using TyMultiplayerCLI;

public class TyPosRot
{
    readonly int[] TY_POS_ROT_OFFSETS = { 0x270B78, 0x270B7C, 0x270B80, 0x271C20 };
    int[] TY_POS_ROT_ADS;
    readonly int[] BP_POS_ROT_OFFSETS = { 0x254268, 0x25426C, 0x270B80, 0x254270 };
    int[] BP_POS_ROT_ADS;
    const int LEVEL_ID_OFFSET = 0x280594;
    int LEVEL_ID_AD;
    public float[] currentPosRot;
    public int currentLevelID;
    public Process tyProcess;
    public IntPtr tyexeHandle => Program.tyexeHandle;


    public TyPosRot()
    {
        currentPosRot = new float[4];
        GetCurrentLevel();
    }

    public int AddOffset(int i)
    {
        return (int)IntPtr.Add(tyProcess.MainModule.BaseAddress, i);
    }

    public void SetMemoryAddresses()
    {
        tyProcess = FindTyexe();
        if (tyProcess == null) { return; }

        //SET MEMORY ADDRESSES USING BASE ADDRESS OF TY APP AND ADDING OFFSETS
        TY_POS_ROT_ADS = new int[]
        { AddOffset(TY_POS_ROT_OFFSETS[0]), AddOffset(TY_POS_ROT_OFFSETS[1]), AddOffset(TY_POS_ROT_OFFSETS[2]), AddOffset(TY_POS_ROT_OFFSETS[3]) };
        BP_POS_ROT_ADS = new int[]
        { AddOffset(BP_POS_ROT_OFFSETS[0]), AddOffset(BP_POS_ROT_OFFSETS[1]), AddOffset(BP_POS_ROT_OFFSETS[2]), AddOffset(BP_POS_ROT_OFFSETS[3]) };
        LEVEL_ID_AD = AddOffset(LEVEL_ID_OFFSET);
    }

    public void GetTyPos()
    {
        if (FindTyexe() != null)
        {
            int bytesRead = 0;
            int[] tempInts;
            GetCurrentLevel();
            if (currentLevelID == 10) { tempInts = BP_POS_ROT_ADS; }
            else { tempInts = TY_POS_ROT_ADS; }
            for(int i = 0; i < 4; i++)
            {
                byte[] buffer = new byte[4];
                Program.ReadProcessMemory((int)tyexeHandle, tempInts[i], buffer, 4, ref bytesRead);
                currentPosRot[i] = BitConverter.ToSingle(buffer, 0);
            }
            return;
        }
    }

    public void GetCurrentLevel()
    {
        int bytesRead = 0;
        byte[] currentLevel = new byte[2];
        Program.ReadProcessMemory((int)tyexeHandle, LEVEL_ID_AD, currentLevel, 2, ref bytesRead);
        currentLevelID = BitConverter.ToInt16(currentLevel, 0);
    }

    public Process FindTyexe()
    {
        foreach (Process p in Process.GetProcesses("."))
        {
            if (p.MainWindowTitle == "TY the Tasmanian Tiger")
            {
                tyProcess = p;
                return p;
            }
        }
        tyProcess = null;
        return null;
    }

}

