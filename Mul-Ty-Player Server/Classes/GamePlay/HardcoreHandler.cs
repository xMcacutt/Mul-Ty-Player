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
            message.AddUShort(_lastClientToDie);
            Server._Server.SendToAll(message);
            _hardcoreRunDead = value;
        }
    }

    private static ushort _lastClientToDie;
    [MessageHandler((ushort)MessageID.HC_RunStatusChanged)]
    public static void HandleRunStatusChanged(ushort fromClientId, Message message)
    {
        _lastClientToDie = fromClientId;
        HardcoreRunDead = message.GetBool();
    }
}