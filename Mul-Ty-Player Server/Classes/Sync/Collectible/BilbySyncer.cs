using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

internal class BilbySyncer : Syncer
{
    public BilbySyncer()
    {
        Name = "Bilby";
        CheckState = 0;
        GlobalObjectData = new Dictionary<int, byte[]>();
        GlobalObjectSaveData = new Dictionary<int, byte[]>();
        GlobalObjectCounts = new Dictionary<int, int>();
        foreach (var i in SyncHandler.MainStages)
        {
            GlobalObjectData.Add(i, Enumerable.Repeat((byte)1, 5).ToArray());
            GlobalObjectSaveData.Add(i, Enumerable.Repeat((byte)0, 5).ToArray());
            GlobalObjectCounts.Add(i, 0);
        }
    }

    public override void HandleServerUpdate(int iLive, int iSave, int level, ushort originalSender)
    {
        if (!GlobalObjectData.ContainsKey(level)) return;
        
        // PREVENT SYNCING OF FINAL BILBY (DEFERRED TO WHEN TE IS COLLECTED)
        if (GlobalObjectCounts[level] == 4 && GlobalObjectData[level][iLive] == 1)
        {
            GlobalObjectSaveData[level][iSave] = 3;
            GlobalObjectCounts[level] = 5;
            return;
        }
        GlobalObjectSaveData[level][iSave] = 1;
        GlobalObjectData[level][iLive] = (byte)CheckState;
        GlobalObjectCounts[level] = GlobalObjectData[level].Count(i => i == CheckState);
        SendUpdatedData(iLive, iSave, level, originalSender);
    }
}

