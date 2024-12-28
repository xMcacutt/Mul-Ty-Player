using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

internal class MushroomSyncer : Syncer
{
    public new Dictionary<int, byte> GlobalObjectSaveData;

    public MushroomSyncer()
    {
        Name = "Mushroom";
        CheckState = 1;
        GlobalObjectSaveData = new Dictionary<int, byte>();
        foreach (var i in SyncHandler.MainStages)
            GlobalObjectSaveData.Add(i, 0x0);
    }

    public override void HandleServerUpdate(int null1, int null2, int level, ushort originalSender)
    {
        //ILIVE AND ISAVE UNNECESSARY
        if (GlobalObjectSaveData[level] == 1) 
            return;
        GlobalObjectSaveData[level] = 1;
        SendUpdatedData(null1, null2, level, originalSender);
    }

    public override void Sync(ushort player)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqCollectibleSync);
        message.AddString(Name);
        message.AddInt(0);
        message.AddBytes(new byte[1]);
        message.AddBytes(GlobalObjectSaveData.Values.ToArray());
        Server._Server.Send(message, player);
    }
}