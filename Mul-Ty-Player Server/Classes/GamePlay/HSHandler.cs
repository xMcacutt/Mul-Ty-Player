using System;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
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
    private static void PlayerCaught(ushort fromClientId, Message message)
    {
        var response = Message.Create(MessageSendMode.Reliable, MessageID.HS_Catch);
        Server._Server.Send(response, message.GetUShort());
    }
    
    public static void StartHideTimer()
    {
        var hideTimer = new Thread(RunTimer);
        hideTimer.Start();
    }

    public static async void RunTimer()
    {
        var countdown = Task.Run(() =>
        {
            for (var i = 10; i > 0; i--)
            {
                if (i is 10 or 30 or 60)
                {
                    var warning = Message.Create(MessageSendMode.Reliable, MessageID.HS_Warning);
                    warning.AddInt(i);
                    Server._Server.SendToAll(warning);
                }
                Task.Delay(1000).Wait();
            }
        });
        await countdown;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_StartSeek);
        Server._Server.SendToAll(message);
    }
}

public enum HSRole
{
    Hider,
    Seeker
}