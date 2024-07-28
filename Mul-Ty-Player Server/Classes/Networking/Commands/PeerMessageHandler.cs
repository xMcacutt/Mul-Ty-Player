using System;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class PeerMessageHandler
{
    [MessageHandler((ushort)MessageID.P2PMessage)]
    public static void Convey(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.P2PMessage);
        var bToAll = message.GetBool();
        var messageText = message.GetString();
        var responseText = bToAll
            ? $"[{DateTime.Now:HH:mm:ss}] {PlayerHandler.Players[fromClientId].Name}: {messageText}"
            : $"[{DateTime.Now:HH:mm:ss}] {PlayerHandler.Players[fromClientId].Name} [WHISPERED]: {messageText}";
        response.AddString(responseText);
        if (bToAll) Server._Server.SendToAll(response);
        else Server._Server.Send(response, message.GetUShort());
    }

    [MessageHandler((ushort)MessageID.Alert)]
    public static void ConveyAlert(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.Alert);
        var messageText = message.GetString();
        var responseText = $"[{DateTime.Now:HH:mm:ss}] {PlayerHandler.Players[fromClientId].Name}: {messageText}";
        response.AddString(responseText);
        Server._Server.SendToAll(response);
    }

    public static void SendMessageToClient(string str, bool printToServer, ushort to)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
        var messageText = $"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}";
        if (to == 65535)
        {
            Console.WriteLine(messageText);
            return;
        }
        message.AddString(messageText);
        Server._Server.Send(message, to);
        if (printToServer) Console.WriteLine(str);
    }

    public static void SendMessageToClients(string str, bool printToServer, ushort except)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
        message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
        Server._Server.SendToAll(message, except);
        if (printToServer) Console.WriteLine(str);
    }

    public static void SendMessageToClients(string str, bool printToServer)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
        message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
        Server._Server.SendToAll(message);
        if (printToServer) Console.WriteLine(str);
    }
}