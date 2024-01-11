using System;
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
    public static int[] BossLevelsWithTriggers = new[] { 7, 19, 15 };
    public static int[] BossTriggerIndices = new[] { 6, 5, 3 };
    public static Dictionary<int, bool> PortalStates;
    public Dictionary<int, int> CurrentLevelsEntredCount;
    public Dictionary<int, int> OldLevelsEntredCount;
    public List<int> InvisPortals;
    public bool _bossPortalsActive;
    
    public LevelLockHandler()
    {
        PortalStates = new Dictionary<int, bool>();
        CurrentLevelsEntredCount = new Dictionary<int, int>();
        OldLevelsEntredCount = new Dictionary<int, int>();
        InvisPortals = new List<int>();
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
        //Console.WriteLine("Requesting Data");
        var message = Message.Create(MessageSendMode.Reliable, MessageID.LL_Sync);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.LL_Sync)]
    public static void ReceiveData(Message message)
    {
        var completed = message.GetInts();
        var active = message.GetInt();
        //Console.WriteLine("Received active level " + active);
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
        //Console.WriteLine("Enabled all levels");
        foreach (var key in PortalStates.Keys.ToList())
        {
            PortalStates[key] = !InvisPortals.Contains(key);
        }
        _bossPortalsActive = true;
        if (Client.HLevel.CurrentLevelId != 0) return;
        var iterator = 0;
        foreach (var level in BossLevelsWithTriggers)
        {
            Client.HSync.HTrigger.SetTriggerActivity(BossTriggerIndices[iterator], true);
            iterator++;
        }
    }
    
    public void DisableAllPortals(int except)
    {
        //Console.WriteLine("Disabled all except " + except);
        foreach (var key in PortalStates.Keys.ToList())
        {
            PortalStates[key] = key == except; 
        }
        _bossPortalsActive = false;
    }

    public void SetPortalStatesInGame()
    {
        if (Client.HLevel.CurrentLevelId != 0)
            return;
        foreach (var level in PortalStates.Keys.ToList())
        {
            ProcessHandler.TryRead(Client.HSync.SyncObjects["Portal"].LiveObjectAddress + 0x9C + 0xB0 * Array.IndexOf(LivePortalOrder, level),
                out byte result, false, "LevelLockHandler: SetPortalStates() 1");
            if ((PortalStates[level] && result < 3) || (!PortalStates[level] && result == 3)) continue;
            var b = PortalStates[level] ? (byte)0x1 : (byte)0x3;
            ProcessHandler.WriteData(Client.HSync.SyncObjects["Portal"].LiveObjectAddress + 0x9C + 0xB0 * Array.IndexOf(LivePortalOrder, level), new byte[] {b}, "LevelLockHandler: SetPortalStates() 2");
        }
        var iterator = 0;
        foreach (var level in BossLevelsWithTriggers)
        {
            var result = Client.HSync.HTrigger.GetTriggerActivity(BossTriggerIndices[iterator]);
            if ((result == 1 && PortalStates[level]) || result == 0 && !PortalStates[level]) continue;
            var b = PortalStates[level] || _bossPortalsActive;
            Client.HSync.HTrigger.SetTriggerActivity(BossTriggerIndices[iterator], b);
            iterator++;
        }
    }

    public void CheckEntry()
    {
        SetPortalStatesInGame();
        foreach (var level in Portals)
        {
            if (OldLevelsEntredCount[level] > 0) continue;
            var address = SyncHandler.SaveDataBaseAddress + 0x70 * level;
            ProcessHandler.TryRead(address, out byte count, false, "LevelLockHandler: CheckEntry()");
            CurrentLevelsEntredCount[level] = count;
            if(OldLevelsEntredCount[level] == 0 && CurrentLevelsEntredCount[level] > 0)
                InformEntry(level);
            OldLevelsEntredCount[level] = CurrentLevelsEntredCount[level];
        }
    }

    private static void InformEntry(int level)
    {
        //Console.WriteLine("Entered level " + level);
        var message = Message.Create(MessageSendMode.Reliable, MessageID.LL_LevelEntered);
        message.AddInt(level);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.LL_LevelEntered)]
    public static void ActiveLevelChanged(Message message)
    {
        //Console.WriteLine("Active level changed in server");
        var level = message.GetInt();
        if (Client.HSync.HLevelLock.InvisPortals.Contains(level))
            Client.HSync.HLevelLock.InvisPortals.Remove(level);
        Client.HSync.HLevelLock.DisableAllPortals(level);
    }
    
    [MessageHandler((ushort)MessageID.LL_LevelCompleted)]
    public static void OpenPortals(Message message)
    {
        //Console.WriteLine("Level completed");
        Client.HSync.HLevelLock.EnableAllCurrentPortals();
    }
    
    [MessageHandler((ushort)MessageID.SetLevelLock)]
    public static void SetLevelLock(Message message)
    {
        SettingsHandler.DoLevelLock = message.GetBool();
        var result = SettingsHandler.DoLevelLock ? "Level lock has been activated" : "Level lock has been disabled";
        Logger.Write(result);
    }
}