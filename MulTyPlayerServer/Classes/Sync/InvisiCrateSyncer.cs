using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

internal class InvisiCrateSyncer : Syncer
{
    public InvisiCrateSyncer()
    {
        Name = "InvisiCrate";
        CheckState = 0;
        GlobalObjectData = new Dictionary<int, byte[]>
        {
            { 0, Enumerable.Repeat((byte)1, 9).ToArray()},
            { 4, Enumerable.Repeat((byte)1, 7).ToArray() },
            { 5, Enumerable.Repeat((byte)1, 6).ToArray() },
            { 6, Enumerable.Repeat((byte)1, 9).ToArray() },
            { 8, Enumerable.Repeat((byte)1, 20).ToArray() },
            { 9, Enumerable.Repeat((byte)1, 24).ToArray() },
            { 10, Enumerable.Repeat((byte)1, 0).ToArray() },
            { 12, Enumerable.Repeat((byte)1, 5).ToArray() },
            { 13, Enumerable.Repeat((byte)1, 29).ToArray() },
            { 14, Enumerable.Repeat((byte)1, 18).ToArray() }
        };
    }

    public override void HandleServerUpdate(int iLive, int iSave, int level, ushort originalSender)
    {
        if (!GlobalObjectData.ContainsKey(level)) return;
        //Console.WriteLine("Sending " + Name + " LiveNumber: " + iLive + " SaveNumber: " + iSave + " For Level: " + level + " To All But: " + Server.PlayerList[originalSender].Name);
        GlobalObjectData[level][iLive] = (byte)CheckState;
        SendUpdatedData(iLive, iSave, level, originalSender);
    }

    public override void Sync(ushort player)
    {
        foreach (var level in GlobalObjectData.Keys)
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqSync);
            message.AddString(Name);
            message.AddInt(level);
            message.AddBytes(GlobalObjectData[level]);
            message.AddBytes(GlobalObjectData[level]);
            Server._Server.Send(message, player);
        }
    }
}