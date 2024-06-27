#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
using MulTyPlayerServer.Classes.Networking.Commands;
using Riptide;

namespace MulTyPlayerServer;

public class HSHandler
{
    //HIDE & SEEK HANDLER
    [MessageHandler((ushort)MessageID.HS_RoleChanged)]
    private static void RoleChanged(ushort fromClientId, Message message)
    {
        if (!PlayerHandler.Players.TryGetValue(fromClientId, out var player))
            return;
        player.Role = (HSRole)message.GetInt();
        AnnounceRoleChanged(fromClientId, player.Role);
    }

    private static void AnnounceRoleChanged(ushort clientId, HSRole role)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_RoleChanged);
        message.AddUShort(clientId);
        message.AddInt((int)role);
        Server._Server.SendToAll(message, clientId);
    }

    [MessageHandler((ushort)MessageID.HS_Catch)]
    private static void HandleCatch(ushort fromClientId, Message message)
    {
        var otherPlayerId = message.GetUShort();
        var otherPlayerRole = (HSRole)message.GetInt();
        var response = Message.Create(MessageSendMode.Reliable, MessageID.HS_Catch);
        response.AddInt((int)otherPlayerRole);
        Server._Server.Send(response, otherPlayerId);

        if (otherPlayerRole == HSRole.Seeker) 
            return;

        var seekerConfirmation = Message.Create(MessageSendMode.Reliable, MessageID.HS_Catch);
        seekerConfirmation.AddInt((int)HSRole.Seeker);
        Server._Server.Send(seekerConfirmation, fromClientId);
    }

    [MessageHandler((ushort)MessageID.HS_Abort)]
    private static void AbortSession(ushort fromClientId, Message message)
    {
        try
        {
            abortTokenSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
            PeerMessageHandler.SendMessageToClient("No Hide & Seek session to abort.", false, fromClientId);
        }
        catch (NullReferenceException)
        {
            PeerMessageHandler.SendMessageToClient("No Hide & Seek session to abort.", false, fromClientId);
        }
    }

    [MessageHandler((ushort)MessageID.HS_Taunt)]
    private static void HandleTaunt(ushort fromClientId, Message message)
    {
        var clientId = message.GetUShort();
        var distance = message.GetFloat();
        var forward = Message.Create(MessageSendMode.Reliable, MessageID.HS_Taunt);
        forward.AddFloat(distance);
        Server._Server.Send(forward, clientId);
    }
    
    public static void StartHideTimer(int hideTimeLength)
    {
        var hideTimer = new Thread(new ParameterizedThreadStart(RunTimer));
        hideTimer.Start(hideTimeLength);
    }

    private static CancellationTokenSource abortTokenSource;
    private static async void RunTimer(object? parameter)
    {
        if (parameter is not int hideTimeLength)
            return;
        
        abortTokenSource = new CancellationTokenSource();
        var abortToken = abortTokenSource.Token;
        
        var countdown = Task.Run(() =>
        {
            abortToken.ThrowIfCancellationRequested();
            for (var i = 5; i > 0; i--)
            {
                if (abortToken.IsCancellationRequested) abortToken.ThrowIfCancellationRequested();
                if (i % 30 == 0 || i == 10)
                {
                    var warning = Message.Create(MessageSendMode.Reliable, MessageID.HS_Warning);
                    warning.AddInt(i);
                    Server._Server.SendToAll(warning);
                }
                Task.Delay(1000, abortToken).Wait(abortToken);
            }
        }, abortToken);
        try
        {
            await countdown;
            var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_StartSeek);
            Server._Server.SendToAll(message);
        }
        catch (OperationCanceledException cancel)
        {
            Console.WriteLine("Hide & Seek session aborted.");
            var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_Abort);
            Server._Server.SendToAll(message);
        }
        finally
        {
            abortTokenSource.Dispose();
        }
    }
}

public enum HSRole
{
    Hider,
    Seeker,
    Spectator
}

public class PerkHandler
{
    [MessageHandler((ushort)MessageID.HS_Freeze)]
    public static void ReceiveFreeze(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.HS_Freeze);
        foreach (var player in PlayerHandler.Players.Values.Where(x => x.Role == HSRole.Hider))
            Server._Server.Send(response, player.ClientID);
    }
}