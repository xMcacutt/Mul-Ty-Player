using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Sync;

public static class LevelLockHandler
{
    public static int ActiveLevel { get; set; }
    private static readonly int[] NormalCompletionLevels = new[] { 4, 5, 6, 8, 9, 10, 12, 13, 14 };
    private static readonly int[] BossLevels = new[] { 7, 19, 15, 20, 23 };
    public static List<int> CompletedLevels { get; set; }

    [MessageHandler((ushort)MessageID.LL_LevelEntered)]
    public static void ClientEnteredLevel(ushort fromClientId, Message message)
    {
        var level = message.GetInt();
        //Console.WriteLine("Player entered level " + level);
        if (CompletedLevels.Contains(level) || (!CompletedLevels.Contains(ActiveLevel) && ActiveLevel != 0)) 
            return;
        //Console.WriteLine("Active level changed to " + level);
        ActiveLevel = level;
        InformEntry(level);
    }

    [MessageHandler((ushort)MessageID.LL_Sync)]
    public static void ReceiveDataReq(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.LL_Sync);
        response.AddInts(CompletedLevels.ToArray());
        response.AddInt(ActiveLevel);
        Server._Server.Send(response, fromClientId);
    }

    private static void InformEntry(int level)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.LL_LevelEntered);
        message.AddInt(level);
        Server._Server.SendToAll(message);
    }

    public static void UpdateCheck()
    {
        if (CompletedLevels.Contains(ActiveLevel) || ActiveLevel == 0) return;
        if (!CheckLevelComplete()) return;
        CompletedLevels.Add(ActiveLevel);
        var message = Message.Create(MessageSendMode.Reliable, MessageID.LL_LevelCompleted);
        Server._Server.SendToAll(message);
    }

    private static bool CheckLevelComplete()
    {
        if (NormalCompletionLevels.Contains(ActiveLevel))
            return SyncHandler.SThEg.GlobalObjectCounts[ActiveLevel] == 8 
                   && SyncHandler.SCog.GlobalObjectCounts[ActiveLevel] == 10;
        if (BossLevels.Contains(ActiveLevel))
            return SyncHandler.SAttribute.GlobalObjectSaveData[
                0x10 + Array.IndexOf(BossLevels, ActiveLevel)] == 1;   
        return true;
    }
}