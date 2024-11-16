using System;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

public class HardcoreHandler
{
    private static bool _hardcoreRunDead;
    public static bool HardcoreRunDead
    {
        get => _hardcoreRunDead;
        set
        {
            if (_hardcoreRunDead == value)
                return;
            var message = Message.Create(MessageSendMode.Reliable, MessageID.HC_RunStatusChanged);
            message.AddBool(value);
            Server._Server.SendToAll(message);
            _hardcoreRunDead = value;
        }
    }
    
    [MessageHandler((ushort)MessageID.HC_RunStatusChanged)]
    public static void HandleRunStatusChanged(ushort fromClientId, Message message)
    {
        HardcoreRunDead = message.GetBool();
    }
}