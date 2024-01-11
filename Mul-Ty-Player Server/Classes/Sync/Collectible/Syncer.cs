using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

internal abstract class Syncer
{
    public Dictionary<int, int> GlobalObjectCounts;

    public Dictionary<int, byte[]> GlobalObjectData;
    public Dictionary<int, byte[]> GlobalObjectSaveData;
    public string Name { get; set; }
    public int CheckState { get; set; }

    public virtual void HandleServerUpdate(int iLive, int iSave, int level, ushort originalSender)
    {
        if (!GlobalObjectData.Keys.Contains(level)) return;
        //Console.WriteLine("Sending " + Name + " LiveNumber: " + iLive + " SaveNumber: " + iSave + " For Level: " + level);
        GlobalObjectData[level][iLive] = (byte)CheckState;
        GlobalObjectSaveData[level][iSave] = (byte)CheckState;
        GlobalObjectCounts[level] = GlobalObjectData[level].Count(i => i == CheckState);
        SendUpdatedData(iLive, iSave, level, originalSender);
    }

    public virtual void SendUpdatedData(int iLive, int iSave, int level, ushort originalSender)
    {
        var syncMessage = SyncMessage.Create(iLive, iSave, level, Name);
        Server._Server.SendToAll(SyncMessage.Encode(syncMessage), originalSender);
    }

    public virtual void Sync(ushort player)
    {
        foreach (var level in GlobalObjectData.Keys)
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqCollectibleSync);
            message.AddString(Name);
            message.AddInt(level);
            message.AddBytes(GlobalObjectData[level]);
            message.AddBytes(GlobalObjectSaveData[level]);
            Server._Server.Send(message, player);
        }
    }
}