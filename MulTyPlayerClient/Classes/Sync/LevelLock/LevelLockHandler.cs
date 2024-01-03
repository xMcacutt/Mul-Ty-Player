﻿using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient.LevelLock;

public class LevelLockHandler
{
    public int[] Portals = new[] { 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 19, 20, 21, 22, 23 };
    public static int[] LivePortalOrder = { 7, 5, 4, 13, 10, 23, 20, 19, 9, 21, 22, 12, 8, 6, 14, 15 };
    public static int[] NonInitiallyVisiblePortals = new[] { 7, 15, 19, 21, 22, 23 };
    public Dictionary<int, bool> PortalStates;
    public Dictionary<int, int> CurrentLevelsEntredCount;
    public Dictionary<int, int> OldLevelsEntredCount;
    public List<int> InvisPortals;
    
    public LevelLockHandler()
    {
        PortalStates = new Dictionary<int, bool>();
        foreach (var portal in Portals)
        {
            CurrentLevelsEntredCount.Add(portal, 0);
            OldLevelsEntredCount.Add(portal, 0);
            PortalStates.Add(portal, !NonInitiallyVisiblePortals.Contains(portal));
        }
        foreach (var portal in NonInitiallyVisiblePortals)
            InvisPortals.Add(portal);
    }

    public void RequestData()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.LL_Sync);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.LL_Sync)]
    public static void ReceiveData(Message message)
    {
        var completed = message.GetInts();
        var active = message.GetInt();
        foreach (var level in completed)
            if (Client.HSync.HLevelLock.InvisPortals.Contains(level))
                Client.HSync.HLevelLock.InvisPortals.Remove(level);
        if (active == 0 || completed.Contains(active))
            Client.HSync.HLevelLock.EnableAllCurrentPortals();
        else
            Client.HSync.HLevelLock.DisableAllPortals(active);
    }

    public void EnableAllCurrentPortals()
    {
        foreach (var key in PortalStates.Keys.ToList())
            PortalStates[key] = !InvisPortals.Contains(key);
    }
    
    public void DisableAllPortals(int except)
    {
        foreach (var key in PortalStates.Keys.ToList())
            PortalStates[key] = key == except;
    }

    public void SetPortalStatesInGame()
    {
        if (Client.HLevel.CurrentLevelId != 0)
            return;
        foreach (var portalIndex in Portals)
        {
            ProcessHandler.TryRead(Client.HSync.SyncObjects["Portal"].LiveObjectAddress + 0x9C + 0xB0 * portalIndex,
                out byte result, false, "LevelLockHandler: SetPortalStates() 1");
            if ((PortalStates[portalIndex] && result > 0) || (!PortalStates[portalIndex] && result == 0)) return;
            ProcessHandler.WriteData(Client.HSync.SyncObjects["Portal"].LiveObjectAddress + 0x9C + 0xB0 * portalIndex,
                BitConverter.GetBytes(PortalStates[portalIndex]), "LevelLockHandler: SetPortalStates() 2");
        }
    }

    public void CheckEntry()
    {
        SetPortalStatesInGame();
        foreach (var level in Portals)
        {
            if (OldLevelsEntredCount[level] > 0) continue;
            
            var address = SyncHandler.SaveDataBaseAddress + 0x70 * level;
            ProcessHandler.TryRead(address, out int count, false, "LevelLockHandler: CheckEntry()");
            CurrentLevelsEntredCount[level] = count;
            
            if(OldLevelsEntredCount[level] == 0 && CurrentLevelsEntredCount[level] > 0)
                InformEntry(level);
            
            OldLevelsEntredCount[level] = CurrentLevelsEntredCount[level];
        }
    }

    private static void InformEntry(int level)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.LL_LevelEntered);
        message.AddInt(level);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.LL_LevelEntered)]
    public static void ActiveLevelChanged(Message message)
    {
        var level = message.GetInt();
        if (Client.HSync.HLevelLock.InvisPortals.Contains(level))
            Client.HSync.HLevelLock.InvisPortals.Remove(level);
        Client.HSync.HLevelLock.DisableAllPortals(level);
    }
    
    [MessageHandler((ushort)MessageID.LL_LevelCompleted)]
    public static void OpenPortals(Message message)
    {
        Client.HSync.HLevelLock.EnableAllCurrentPortals();
    }
}