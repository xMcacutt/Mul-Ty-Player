using System;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

public class ChaosHandler
{
    public int ChaosSeed;
    private static Random _chaosRand = new Random();

    public ChaosHandler()
    {
        ChaosSeed = _chaosRand.Next();
    }
    
    [MessageHandler((ushort)MessageID.CH_Shuffle)]
    public static void RequestShuffle(ushort fromClientId, Message message)
    {
        Program.HChaos.ChaosSeed = _chaosRand.Next();
        var response = Message.Create(MessageSendMode.Reliable, MessageID.CH_Shuffle);
        response.AddInt(Program.HChaos.ChaosSeed);
        Server._Server.SendToAll(response);
    }
}