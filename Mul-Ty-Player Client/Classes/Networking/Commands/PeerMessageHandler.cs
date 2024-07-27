using System;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public static class PeerMessageHandler
{
    public static void SendMessage(string text, ushort? toClientId = null)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.P2PMessage);
        message.AddBool(toClientId == null);
        message.AddString(text);
        if (toClientId != null) message.AddUShort((ushort)toClientId);
        Client._client.Send(message);
    }
    
    [MessageHandler((ushort)MessageID.Alert)]
    public static void ConveyAlert(Message message)
    {
        SFXPlayer.PlaySound(SFX.Alert);
        Logger.Write(message.GetString());
    }
    
    [MessageHandler((ushort)MessageID.P2PMessage)]
    public static void HandleMessageFromPeer(Message message)
    {
        Logger.Write(message.GetString());
    }
}