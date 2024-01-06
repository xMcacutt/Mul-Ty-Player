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
        if (!GlobalObjectData.Keys.Contains(level)) return;
        Console.WriteLine("Sending " + Name + " LiveNumber: " + iLive + " SaveNumber: " + iSave + " For Level: " + level);
        GlobalObjectData[level][iLive] = (byte)CheckState;
        GlobalObjectCounts[level] = GlobalObjectData[level].Count(i => i == CheckState);
        Console.WriteLine(GlobalObjectCounts[level]);
        GlobalObjectSaveData[level][iSave] = GlobalObjectCounts[level] == 5 ? (byte)3 : (byte)1;
        if (GlobalObjectCounts[level] == 5 && SettingsHandler.Settings.DoSyncTEs)
            SendTESpawnMessage(level, originalSender);
        SendUpdatedData(iLive, iSave, level, originalSender);
    }

    private void SendTESpawnMessage(int level, ushort originalSender)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SpawnBilbyTE);
        message.AddInt(level);
        Server._Server.SendToAll(message, originalSender);
    }
}