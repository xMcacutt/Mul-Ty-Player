using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualBasic.Logging;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandRequestHost : Command
{
    public MtpCommandRequestHost()
    {
        Name = "requesthost";
        Aliases = new List<string> { "rh", "reqhost" };
        HostOnly = false;
        SpectatorAllowed = true;
        Usages = new List<string> { "/requesthost" };
        Description = "Request host privileges if no host already exists.";
        ArgDescriptions = new Dictionary<string, string>();
    }

    public override void InitExecute(string[] args)
    {
        if (args.Length != 0)
        {
            SuggestHelp();
            return;
        }
        if (PlayerHandler.TryGetPlayer(Client._client.Id, out var p) && p.IsHost)
        {
            LogError("You already have host privileges");
            return;
        }
        RequestHost();
    }
    
    private static void RequestHost()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqHost);
        Client._client.Send(message);
    }
    
    [MessageHandler((ushort)MessageID.ReqHost)]
    public static void RequestHostResponse(Message message)
    {
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
        {
            Logger.Write("[ERROR] Could not find own player in the list.");
            return;
        }
        var makeHost = message.GetBool();
        if (makeHost)
        {
            self.IsHost = true;
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(ModelController.Lobby.UpdateHost));
            Logger.Write("You have been made host. You now have access to host only commands.");
            return;
        }
        try
        {
            var existingHost = PlayerHandler.Players.First(p => p.IsHost);
            Logger.Write($"Player {existingHost.Name} already has host privileges");
        }
        catch
        {
            Logger.Write("Existing host found by server, not by client. Denying host privileges.");
            
        }
    }
    
    [MessageHandler((ushort)MessageID.HostChange)]
    public static void HostChange(Message message)
    {
        var newHostId = message.GetUShort();
        foreach (var player in PlayerHandler.Players) player.IsHost = false;
        if (!PlayerHandler.TryGetPlayer(newHostId, out var newHost))
        {
            Logger.Write("[ERROR] Could not find new host in player list.");
            return;
        }
        newHost.IsHost = true;
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.UpdateHost));
    }
}