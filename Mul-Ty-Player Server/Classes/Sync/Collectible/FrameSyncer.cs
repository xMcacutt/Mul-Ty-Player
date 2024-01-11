using System;
using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

internal class FrameSyncer : Syncer
{
    public byte[] FrameSaveData;

    public FrameSyncer()
    {
        Name = "Frame";
        CheckState = 1;
        FrameSaveData = new byte[0x2E];
        GlobalObjectData = new Dictionary<int, byte[]>
        {
            { 0, new byte[9] },
            { 4, new byte[7] },
            { 5, new byte[6] },
            { 6, new byte[9] },
            { 8, new byte[20] },
            { 9, new byte[24] },
            { 12, new byte[5] },
            { 13, new byte[29] },
            { 14, new byte[18] },
            { 21, new byte[123] },
            { 22, new byte[123] }
        };
    }

    public override void HandleServerUpdate(int iLive, int iSave, int level, ushort originalSender)
    {
        if (!GlobalObjectData.ContainsKey(level)) return;
        GlobalObjectData[level][iLive] = (byte)CheckState;
        var byteIndex = iSave / 8;
        var bitOffset = iSave % 8;
        FrameSaveData[byteIndex] |= (byte)(1 << bitOffset);
        SendUpdatedData(iLive, iSave, level, originalSender);
    }

    public override void Sync(ushort player)
    {
        foreach (var level in GlobalObjectData.Keys)
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqCollectibleSync);
            message.AddString(Name);
            message.AddInt(level);
            message.AddBytes(GlobalObjectData[level]);
            message.AddBytes(Array.Empty<byte>());
            Server._Server.Send(message, player);
        }

        var saveDataMessage = Message.Create(MessageSendMode.Reliable, MessageID.ReqCollectibleSync);
        saveDataMessage.AddString(Name);
        saveDataMessage.AddInt(373);
        saveDataMessage.AddBytes(Array.Empty<byte>());
        saveDataMessage.AddBytes(FrameSaveData);
        Server._Server.Send(saveDataMessage, player);
    }
}