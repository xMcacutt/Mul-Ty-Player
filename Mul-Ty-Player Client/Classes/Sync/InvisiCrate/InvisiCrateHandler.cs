﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;

namespace MulTyPlayerClient;

internal class InvisiCrateHandler : SyncObjectHandler
{
    public InvisiCrateHandler()
    {
        Name = "InvisiCrate";
        WriteState = 2;
        CheckState = 2;
        CounterAddress = PointerCalculations.GetPointerAddress(0x0028A8E8, new[] { 0x390 });
        CounterAddressStatic = false;
        CounterByteLength = 4;
        CurrentObjectData = Enumerable.Repeat((byte)1, 300).ToArray();
        PreviousObjectData = Enumerable.Repeat((byte)1, 300).ToArray();
        GlobalObjectData = new Dictionary<int, byte[]>
        {
            { 0, Enumerable.Repeat((byte)1, 9).ToArray() },
            { 4, Enumerable.Repeat((byte)1, 7).ToArray() },
            { 5, Enumerable.Repeat((byte)1, 6).ToArray() },
            { 6, Enumerable.Repeat((byte)1, 9).ToArray() },
            { 8, Enumerable.Repeat((byte)1, 20).ToArray() },
            { 9, Enumerable.Repeat((byte)1, 24).ToArray() },
            { 10, Enumerable.Repeat((byte)1, 0).ToArray() },
            { 12, Enumerable.Repeat((byte)1, 5).ToArray() },
            { 13, Enumerable.Repeat((byte)1, 29).ToArray() },
            { 14, Enumerable.Repeat((byte)1, 18).ToArray() },
            { 21, Enumerable.Repeat((byte)1, 25).ToArray() },
            { 22, Enumerable.Repeat((byte)1, 20).ToArray() }
        };
        LiveSync = new LiveInvisiCrateSyncer(this);
        SetSyncClasses(LiveSync);
    }

    public override void HandleClientUpdate(int iLive, int iSave, int level)
    {
        if (GlobalObjectData[level][iLive] == 2) return;
        GlobalObjectData[level][iLive] = (byte)CheckState;
        if (level != Client.HLevel.CurrentLevelId) return;
        LiveSync.Collect(iLive);
    }

    public override void Sync(int level, byte[] liveData, byte[] saveData)
    {
        var crateCount = GlobalObjectData[level].Length;
        for (var i = 0; i < crateCount; i++)
            if (liveData[i] == 2 && GlobalObjectData[level][i] == 1)
                GlobalObjectData[level][i] = 0;
        if (Client.HLevel.CurrentLevelId != level) 
            return;
        PreviousObjectData = liveData;
        CurrentObjectData = liveData;
        LiveSync.Sync(liveData, crateCount, CheckState);
    }

    public override bool CheckObserverCondition(byte previousState, byte currentState)
    {
        return previousState != 2 && currentState == 2;
    }

    public override void SetMemAddrs()
    {
        CounterAddress = PointerCalculations.GetPointerAddress(0x28A8E8, new[] { 0x390 });
        LiveObjectAddress = PointerCalculations.GetPointerAddress(0x254DB0, new[] { 0x0 });
        ProcessHandler.CheckAddress(LiveObjectAddress, (ushort)0xB098, "Crate base address check");
    }

    public override void CheckObserverChanged()
    {
        if (Levels.GetLevelData(Client.HLevel.CurrentLevelId).FrameCount == 0)
            return;

        ObserverState = ReadObserver(CounterAddress, CounterByteLength);

        if (PreviousObserverState == ObserverState || ObserverState == 0)
            return;

        PreviousObserverState = ObserverState;
        CurrentObjectData = LiveSync.ReadData();
        var crateCount = GlobalObjectData[Client.HLevel.CurrentLevelId].Length;
        if (PreviousObjectData.Length != crateCount) return;
        for (var iLive = 0; iLive < crateCount; iLive++)
            if (CheckObserverCondition(PreviousObjectData[iLive], CurrentObjectData[iLive]))
            {
                PreviousObjectData[iLive] = CurrentObjectData[iLive] = WriteState;
                if (GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] != CheckState)
                {
                    GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] = (byte)CheckState;
                    Client.HSync.SendDataToServer(iLive, iLive, Client.HLevel.CurrentLevelId, Name);
                }
            }
    }
}