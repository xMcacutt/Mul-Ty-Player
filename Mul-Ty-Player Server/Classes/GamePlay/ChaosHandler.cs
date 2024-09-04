using System;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

public class ChaosHandler
{
    public int ChaosSeed;
    public bool ShuffleOnStart;
    private static Random _chaosRand = new Random();

    public ChaosHandler()
    {
        ChaosSeed = _chaosRand.Next();
    }

    public static void ForceShuffle()
    {
        Program.HChaos.ChaosSeed = _chaosRand.Next();
        var response = Message.Create(MessageSendMode.Reliable, MessageID.CH_Shuffle);
        response.AddInt(Program.HChaos.ChaosSeed);
        response.AddBool(false);
        Server._Server.SendToAll(response);
    }
    
    [MessageHandler((ushort)MessageID.CH_Shuffle)]
    public static void RequestShuffle(ushort fromClientId, Message message)
    {
        ForceShuffle();
    }

    [MessageHandler((ushort)MessageID.CH_SetSeed)]
    public static void RequestSeed(ushort fromClientId, Message message)
    {
        Program.HChaos.ChaosSeed = message.GetInt();
        var response = Message.Create(MessageSendMode.Reliable, MessageID.CH_Shuffle);
        response.AddInt(Program.HChaos.ChaosSeed);
        response.AddBool(true);
        Server._Server.SendToAll(response);
    }
    
    [MessageHandler((ushort)MessageID.CH_ShuffleOnStartToggle)]
    public static void HandleRequestShuffleOnStartToggle(ushort fromClientId, Message message)
    {
        Program.HChaos.ShuffleOnStart = !Program.HChaos.ShuffleOnStart;
        var response = Message.Create(MessageSendMode.Reliable, MessageID.CH_ShuffleOnStartToggle);
        response.AddBool(Program.HChaos.ShuffleOnStart);
        Server._Server.SendToAll(response);
    }
}