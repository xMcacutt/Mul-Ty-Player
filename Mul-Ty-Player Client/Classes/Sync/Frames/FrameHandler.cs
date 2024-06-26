﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

internal class FrameHandler : SyncObjectHandler
{
    public int CrateFrameAddress;
    public int NonCrateFrameAddress;
    public (int, byte)[] FrameData;
    public (int, byte)[] OldFrameData;
    public int InvisiCrateAddress;
    public byte[] NullState;
    public new byte[] ObserverState;
    public new byte[] PreviousObserverState;

    public FrameHandler()
    {
        Name = "Frame";
        WriteState = 1;
        CheckState = 1;
        SeparateID = true;
        IDOffset = 0x84;
        CounterByteLength = 0x2F;
        ObserverState = new byte[0x2F];
        PreviousObserverState = new byte[0x2F];
        PreviousObjectData = new byte[200];
        CurrentObjectData = new byte[200];
        LiveSync = new LiveFrameSyncer(this);
        SaveSync = new SaveFrameSyncer();
        SetSyncClasses(LiveSync, SaveSync);
        GlobalObjectData = new Dictionary<int, byte[]>();
        foreach (var ld in Levels.MainStages)
        {
            if (ld.Id == 10) continue;
            GlobalObjectData.Add(ld.Id, new byte[ld.FrameCount]);
        }

        GlobalObjectData.Add(Levels.RainbowCliffs.Id, new byte[Levels.RainbowCliffs.FrameCount]);
        GlobalObjectData.Add(Levels.BonusWorld1.Id, new byte[Levels.BonusWorld1.FrameCount]);
        GlobalObjectData.Add(Levels.BonusWorld2.Id, new byte[Levels.BonusWorld2.FrameCount]);
    }

    public new byte[] ReadObserver(int address, int size)
    {
        ProcessHandler.TryReadBytes(address, out var result, size, false);
        return result;
    }

    public override void CheckObserverChanged()
    {
        ObserverState = ReadObserver(CounterAddress, CounterByteLength);
        if (PreviousObserverState.SequenceEqual(ObserverState) || ObserverState.All(b => b == 0)) 
            return;
        Array.Copy(ObserverState, PreviousObserverState, 0x2F);
        
        // FRAME DATA ITEM 1 IS THE INDEX OF THE FRAME
        FrameData = (LiveSync as LiveFrameSyncer)?.ReadData();
        if (FrameData == null)
            return;
        CurrentObjectData = FrameData.Select(x => x.Item2).ToArray();
        for (var iLive = 0; iLive < CurrentObjectData.Length; iLive++)
        {
            if (!CheckObserverCondition(PreviousObjectData[iLive], CurrentObjectData[iLive])) 
                continue;

            //FRAME IS COLLECTED
            PreviousObjectData[iLive] = CurrentObjectData[iLive] = WriteState;
            var iSave = FrameData[iLive].Item1;
            if (GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] == CheckState) 
                continue;
            //Logger.Write(Name + " number " + iLive + " collected.");
            GlobalObjectData[Client.HLevel.CurrentLevelId][iLive] = (byte)CheckState;
            Client.HSync.SendDataToServer(iLive, iSave, Client.HLevel.CurrentLevelId, Name);
        }
    }

    public override void Sync(int level, byte[] liveData, byte[] saveData)
    {
        if (level == 373)
        {
            (SaveSync as SaveFrameSyncer)?.Sync(saveData);
            return;
        }

        var objectAmount = Levels.GetLevelData(level).FrameCount;
        for (var i = 0; i < objectAmount; i++)
            if (liveData[i] == CheckState && GlobalObjectData[level][i] != CheckState)
                GlobalObjectData[level][i] = WriteState;
        if (Client.HLevel.CurrentLevelId == level)
        {
            LiveSync.Sync(liveData, objectAmount, CheckState);
            PreviousObjectData = liveData;
            CurrentObjectData = liveData;
        }
    }

    public override void SetMemAddrs()
    {
        CrateFrameAddress = PointerCalculations.GetPointerAddress(0x274D24, new[] { 0x1C4, 0x0 });
        NonCrateFrameAddress = PointerCalculations.GetPointerAddress(0x26BEB0, new[] { 0x0 });
        CounterAddress = SyncHandler.SaveDataBaseAddress + 0xAC2;
    }

    public override bool CheckObserverCondition(byte previousState, byte currentState)
    {
        return previousState == 0 && currentState == 1;
    }
}