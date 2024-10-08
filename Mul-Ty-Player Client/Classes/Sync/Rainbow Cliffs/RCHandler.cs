﻿using System;
using System.Collections.Generic;

namespace MulTyPlayerClient;

internal class RCHandler : SyncObjectHandler
{
    public new Dictionary<int, byte> GlobalObjectData;

    public RCHandler()
    {
        Name = "RC";
        CheckState = 1;
        SaveState = 1;
        CounterAddress = SyncHandler.SaveDataBaseAddress + 0xA84;
        CounterAddressStatic = false;
        SaveSync = new SaveRCSyncer();
        SetSyncClasses(SaveSync);
        GlobalObjectData = new Dictionary<int, byte>();
        foreach (int i in Enum.GetValues(typeof(RCData))) GlobalObjectData.Add(i, 0);
    }

    public override void CheckObserverChanged()
    {
        var dataAmount = 12;
        for (var i = 0; i < dataAmount; i++)
        {
            ProcessHandler.TryRead(CounterAddress + i, out byte dataState, false, "RCHandler::CheckObserverChanged()");
            if (dataState == 1 && GlobalObjectData[i] == 0)
            {
                GlobalObjectData[i] = 1;
                Client.HSync.SendDataToServer(0, i, 0, Name);
            }
        }
    }

    public override void SetMemAddrs()
    {
        CounterAddress = SyncHandler.SaveDataBaseAddress + 0xA84;
    }

    public override void Sync(int null1, byte[] null2, byte[] saveData)
    {
        var dataAmount = 12;

        SaveSync.Sync(0, saveData);

        for (var i = 0; i < dataAmount; i++)
            if (saveData[i] == 1)
                GlobalObjectData[i] = 1;
    }

    public override void HandleClientUpdate(int null1, int iAttribute, int null2)
    {
        GlobalObjectData[iAttribute] = 1;
        SaveSync.Save(iAttribute, null);
    }
}