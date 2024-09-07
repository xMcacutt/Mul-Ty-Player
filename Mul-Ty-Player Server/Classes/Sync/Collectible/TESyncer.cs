using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
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
            GlobalObjectSaveData.Add(i, Enumerable.Repeat((byte)1, 8).ToArray());
            GlobalObjectCounts.Add(i, 0);
        }
    }

    public override void HandleServerUpdate(int iLive, int iSave, int level, ushort originalSender)
    {
        if (!GlobalObjectData.ContainsKey(level)) return;
        GlobalObjectData[level][iLive] = (byte)CheckState;
        GlobalObjectSaveData[level][iSave] = (byte)CheckState;
        GlobalObjectCounts[level] = GlobalObjectData[level].Count(i => i == CheckState);
        SendUpdatedData(iLive, iSave, level, originalSender);
        switch (iSave)
        {
            // IF TE IS BILBY TE, SEND MESSAGE TO CLIENTS TO ALSO DESPAWN LAST BILBY
            case 1 when SettingsHandler.ServerSettings.DoSyncBilbies:
            {
                ((BilbySyncer)Program.HSync.Syncers["Bilby"]).GlobalObjectData[level] = new byte[5];
                var bilbyMessage = Message.Create(MessageSendMode.Reliable, MessageID.DespawnAllBilbies);
                bilbyMessage.AddInt(level);
                Server._Server.SendToAll(bilbyMessage);
                break;
            }
            // IF TE IS MAIN OBJECTIVE TE, SEND MESSAGE TO CLIENTS TO SPAWN STOPWATCH FOR TA
            case 3:
            {
                var stopWatchMessage = Message.Create(MessageSendMode.Reliable, MessageID.StopWatch);
                stopWatchMessage.AddInt(level);
                Server._Server.SendToAll(stopWatchMessage);
                break;
            }
        }
    }
}