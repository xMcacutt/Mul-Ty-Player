#nullable enable
using System;
using System.Linq;
using System.Numerics;
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

    private static void RunRadiusCheck()
    {
        foreach (var seeker in PlayerHandler.Players.Values.Where(x => x.Role == HSRole.Seeker))
        {
            foreach (var hider in PlayerHandler.Players.Values.Where(x => x.Role == HSRole.Hider))
            {
                var seekerVector = new Vector3(seeker.Coordinates[0], seeker.Coordinates[1], seeker.Coordinates[2]);
                var hiderVector = new Vector3(hider.Coordinates[0], hider.Coordinates[1], hider.Coordinates[2]);
                var radiusCheckDistance = seeker.CurrentLevel == 10
                    ? SettingsHandler.HSRange * 1.25
                    : SettingsHandler.HSRange;
                if (Vector3.Distance(hiderVector, seekerVector) > radiusCheckDistance)
                    continue;
                var catchMessage = Message.Create(MessageSendMode.Reliable, MessageID.HS_Catch);
                Server._Server.Send(catchMessage, seeker.ClientID);
                Server._Server.Send(catchMessage, hider.ClientID);
            }
        }
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
        
        var hideTime = Task.Run(() =>
        {
            abortToken.ThrowIfCancellationRequested();
            for (var i = hideTimeLength; i > 0; i--)
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

        var seekTime = Task.Run(() =>
        {
            while(PlayerHandler.Players.Values.Any(x => x.Role == HSRole.Hider))
                RunRadiusCheck();
        }, abortToken);
            
        try
        {
            await hideTime;
            var startMessage = Message.Create(MessageSendMode.Reliable, MessageID.HS_StartSeek);
            Server._Server.SendToAll(startMessage);
            
            await seekTime;
            var endMessage = Message.Create(MessageSendMode.Reliable, MessageID.HS_EndSeek);
            Server._Server.SendToAll(endMessage);
        }
        catch (OperationCanceledException cancel)
        {
            Console.WriteLine("Hide & Seek session aborted.");
            var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_Abort);
            Server._Server.SendToAll(message);
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