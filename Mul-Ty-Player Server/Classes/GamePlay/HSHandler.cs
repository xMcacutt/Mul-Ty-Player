#nullable enable
using System;
using System.Collections.Generic;
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
    public static HSMode Mode = HSMode.Neutral;
    
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
        Mode = HSMode.Neutral;
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
                hider.Role = HSRole.Seeker;
                var catchMessage = Message.Create(MessageSendMode.Reliable, MessageID.HS_Catch);
                Server._Server.Send(catchMessage, seeker.ClientID);
                catchMessage = Message.Create(MessageSendMode.Reliable, MessageID.HS_Catch);
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

        Mode = HSMode.HideTime;
        var hideTime = Task.Run(() =>
        {
            abortToken.ThrowIfCancellationRequested();
            // CHANGE THIS FOR LOWER HIDESEEK TIME
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
            await hideTime;
            var startMessage = Message.Create(MessageSendMode.Reliable, MessageID.HS_StartSeek);
            Server._Server.SendToAll(startMessage);
        }
        catch (OperationCanceledException cancel)
        {
            Console.WriteLine("Hide & Seek session aborted.");
            var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_Abort);
            Server._Server.SendToAll(message);
        }
        
        Mode = HSMode.SeekTime;
        var seekTime = Task.Run(async () =>
        {
            while (PlayerHandler.Players.Values.Any(x => x.Role == HSRole.Hider))
            {
                RunRadiusCheck();
                await Task.Delay(75);
            }
        }, abortToken);
        try
        {
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
        Mode = HSMode.Neutral;
    }
    
    [MessageHandler((ushort)MessageID.HS_ForceRole)]
    public static void HandleForceRoleRequest(ushort fromClientId, Message message)
    {
        var clientId = message.GetUShort();
        if (!PlayerHandler.Players.TryGetValue(clientId, out var player)) 
            return;
        if (player.Role == HSRole.Spectator)
            return;
        player.Role = player.Role == HSRole.Hider ? HSRole.Seeker : HSRole.Hider;
        var response = Message.Create(MessageSendMode.Reliable, MessageID.HS_RoleChanged);
        response.AddUShort(clientId);
        response.AddInt((int)player.Role);
        Server._Server.Send(response, clientId);
    }
    
}

public enum HSRole
{
    Hider,
    Seeker,
    Spectator
}

public enum HSMode
{
    HideTime,
    SeekTime,
    Neutral,
}

public class PerkHandler
{
    [MessageHandler((ushort)MessageID.HS_Freeze)]
    public static void ReceiveFreeze(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.HS_Freeze);
        foreach (var player in PlayerHandler.Players.Values.Where(x => x.Role != PlayerHandler.Players[fromClientId].Role && x.Role != HSRole.Spectator))
            Server._Server.Send(response, player.ClientID);
    }
    
    [MessageHandler((ushort)MessageID.HS_Flashbang)]
    public static void ReceiveFlashbang(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.HS_Flashbang);
        foreach (var player in PlayerHandler.Players.Values.Where(x => x.Role != PlayerHandler.Players[fromClientId].Role && x.Role != HSRole.Spectator))
            Server._Server.Send(response, player.ClientID);
    }
}