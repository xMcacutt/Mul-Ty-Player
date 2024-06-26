﻿using System;

namespace MulTyPlayerClient;

internal class LiveOpalSyncer : LiveDataSyncer
{
    public LiveOpalSyncer(OpalHandler HOpal)
    {
        HSyncObject = HOpal;
    }

    private OpalHandler HOpal => HSyncObject as OpalHandler;
    private static LevelHandler HLevel => Client.HLevel;

    public override byte[] ReadData()
    {
        // Get the number of crate opals in the current level
        var crateOpalsInLevel = Levels.GetLevelData(HLevel.CurrentLevelId).CrateOpalCount;

        // Create an array to store the current opals
        var currentOpals = new byte[300];

        // Read the non-crate opals from memory
        var address = HOpal.NonCrateOpalsAddress;
        for (var i = 0; i < 300 - crateOpalsInLevel; i++)
            ProcessHandler.TryRead(address + 0x78 + 0x114 * i, out currentOpals[i], false,
                "LiveOpalSyncer::ReadData() {1}");

        // If there are no crate opals in the level, return the current opals array
        if (crateOpalsInLevel == 0) return currentOpals;

        // Read the crate opals from memory and add them to the current opals array
        address = HOpal.CrateOpalsAddress;
        for (var i = 0; i < crateOpalsInLevel; i++)
            ProcessHandler.TryRead(address + 0x78 + 0x114 * i, out currentOpals[300 - crateOpalsInLevel + i], false,
                "LiveOpalSyncer::ReadData() {2}");

        return currentOpals;
    }

    public override void Collect(int index)
    {
        if (Client.HGameState.IsOnMainMenuOrLoading) 
            return;
        
        int baseAddress;
        var crateOpalsInCurrentLevel = Levels.GetLevelData(HLevel.CurrentLevelId).CrateOpalCount;
        int address;
        if (HLevel.CurrentLevelId == 10)
        {
            baseAddress = HOpal.CrateOpalsAddress;
        }
        else if (index < 300 - crateOpalsInCurrentLevel)
        {
            baseAddress = HOpal.NonCrateOpalsAddress;
        }
        else
        {
            var nonCrateOpalsInCurrentLevel = 300 - crateOpalsInCurrentLevel;
            address = HOpal.CrateOpalsAddress + 0x78 + 0x114 * (index - nonCrateOpalsInCurrentLevel);
            ProcessHandler.TryRead(address, out HOpal.CurrentObjectData[index], false, "LiveOpalSyncer::Collect() {1}");
            if (HOpal.CurrentObjectData[index] >= 3) return;
            ProcessHandler.WriteData(HOpal.CrateOpalsAddress + 0x78 + 0x114 * (index - nonCrateOpalsInCurrentLevel),
                BitConverter.GetBytes(3), $"Collecting opal {index}");
            return;
        }

        address = baseAddress + 0x78 + 0x114 * index;
        ProcessHandler.TryRead(address, out HOpal.CurrentObjectData[index], false, "LiveOpalSyncer::Collect() {1}");
        if (HOpal.CurrentObjectData[index] >= 3) return;
        ProcessHandler.WriteData(address, BitConverter.GetBytes(3), $"Collecting opal {index}");
    }
}