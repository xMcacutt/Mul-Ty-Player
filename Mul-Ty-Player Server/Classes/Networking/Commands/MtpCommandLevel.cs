using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandLevel
{
    [MessageHandler((ushort)MessageID.ForceLevelChange)]
    private static void HandleForceLevelChange(ushort fromClientId, Message message)
    {
        var level = message.GetInt();
        var response = Message.Create(MessageSendMode.Reliable, MessageID.ForceLevelChange);
        response.AddInt(level);
        Server._Server.SendToAll(response, fromClientId);
    }
}