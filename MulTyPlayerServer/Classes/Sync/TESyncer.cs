using System.Collections.Generic;
using System.Linq;
using Riptide;

namespace MulTyPlayerServer.Classes.Sync;

internal class TESyncer : Syncer
{
    public TESyncer()
    {
        Name = "TE";
        CheckState = 5;
        GlobalObjectData = new Dictionary<int, byte[]>();
        GlobalObjectSaveData = new Dictionary<int, byte[]>();
        GlobalObjectCounts = new Dictionary<int, int>();
        foreach (var i in SyncHandler.MainStages)
        {
            GlobalObjectData.Add(i, Enumerable.Repeat((byte)1, 8).ToArray());
            GlobalObjectSaveData.Add(i, Enumerable.Repeat((byte)1, 10).ToArray());
            GlobalObjectCounts.Add(i, 0);
        }
    }

    public override void HandleServerUpdate(int iLive, int iSave, int level, ushort originalSender)
    {
        if (!GlobalObjectData.ContainsKey(level)) return;
        //Console.WriteLine("Sending " + Name + " LiveNumber: " + iLive + " SaveNumber: " + iSave + " For Level: " + level);
        GlobalObjectData[level][iLive] = (byte)CheckState;
        GlobalObjectSaveData[level][iSave] = (byte)CheckState;
        GlobalObjectCounts[level] = GlobalObjectData[level].Count(i => i == CheckState);
        SendUpdatedData(iLive, iSave, level, originalSender);
        if (iSave != 3) return;
        var stopWatchActivateMessage = Message.Create(MessageSendMode.Reliable, MessageID.StopWatch);
        stopWatchActivateMessage.AddInt(level);
        Server._Server.SendToAll(stopWatchActivateMessage, originalSender);
    }
}